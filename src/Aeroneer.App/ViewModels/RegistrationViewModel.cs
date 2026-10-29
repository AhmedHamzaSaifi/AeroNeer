using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Aeroneer.Core.Models;
using Aeroneer.Core.Validation;

namespace Aeroneer.App.ViewModels;

public sealed class RegistrationViewModel : INotifyPropertyChanged
{
    private readonly ICandidateRepository _candidateRepository;
    private string? _companyOrInstitute;
    private string? _firstName;
    private string? _lastName;
    private string? _address;
    private string? _zipCode;
    private string? _place;
    private CountryOption? _selectedCountry;
    private DateTime? _birthDate;
    private string? _gender;
    private string? _telephone;
    private string? _email;
    private string _statusMessage = string.Empty;
    private bool _isSaving;

    public RegistrationViewModel(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
        Countries = CountryOptions.GetAll();
        Genders = ["Female", "Male", "Non-binary", "Prefer not to say", "Self-describe"];
        SaveCommand = new AsyncCommand(SaveAsync, CanSave);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public IReadOnlyList<CountryOption> Countries { get; }
    public IReadOnlyList<string> Genders { get; }
    public ICommand SaveCommand { get; }
    public string StatusMessage { get => _statusMessage; private set => Set(ref _statusMessage, value); }
    public bool IsSaving { get => _isSaving; private set => Set(ref _isSaving, value); }

    public string? CompanyOrInstitute { get => _companyOrInstitute; set => Set(ref _companyOrInstitute, value); }
    public string? FirstName { get => _firstName; set { if (Set(ref _firstName, value)) RefreshValidation(); } }
    public string? LastName { get => _lastName; set { if (Set(ref _lastName, value)) RefreshValidation(); } }
    public string? Address { get => _address; set => Set(ref _address, value); }
    public string? ZipCode { get => _zipCode; set => Set(ref _zipCode, value); }
    public string? Place { get => _place; set => Set(ref _place, value); }
    public CountryOption? SelectedCountry { get => _selectedCountry; set { if (Set(ref _selectedCountry, value)) RefreshValidation(); } }
    public DateTime? BirthDate { get => _birthDate; set { if (Set(ref _birthDate, value)) RefreshValidation(); } }
    public string? Gender { get => _gender; set => Set(ref _gender, value); }
    public string? Telephone { get => _telephone; set => Set(ref _telephone, value); }
    public string? Email { get => _email; set { if (Set(ref _email, value)) RefreshValidation(); } }

    private bool CanSave() => !IsSaving && CandidateRegistrationValidator.Validate(CreateRequest()).Count == 0;

    private async Task SaveAsync()
    {
        IsSaving = true;
        StatusMessage = string.Empty;
        UpdateSaveCommand();
        try
        {
            var request = CreateRequest();
            var errors = CandidateRegistrationValidator.Validate(request);
            if (errors.Count != 0)
            {
                StatusMessage = string.Join(" ", errors.Select(error => error.Message));
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var candidate = new CandidateProfile(
                Guid.NewGuid(),
                Normalize(CompanyOrInstitute),
                FirstName!.Trim(),
                LastName!.Trim(),
                Normalize(Address),
                Normalize(ZipCode),
                Normalize(Place),
                SelectedCountry!.Code,
                DateOnly.FromDateTime(BirthDate!.Value),
                Normalize(Gender),
                Normalize(Telephone),
                Email!.Trim(),
                now,
                now);
            await _candidateRepository.SaveAsync(candidate, CancellationToken.None);
            StatusMessage = "Candidate profile saved to the local database.";
        }
        catch (Exception)
        {
            StatusMessage = "The profile could not be saved. Check the local database connection and try again.";
        }
        finally
        {
            IsSaving = false;
            UpdateSaveCommand();
        }
    }

    private CandidateRegistrationRequest CreateRequest() => new(
        CompanyOrInstitute,
        FirstName,
        LastName,
        Address,
        ZipCode,
        Place,
        SelectedCountry?.Code,
        BirthDate is null ? null : DateOnly.FromDateTime(BirthDate.Value),
        Gender,
        Telephone,
        Email);

    private void UpdateSaveCommand() => ((AsyncCommand)SaveCommand).NotifyCanExecuteChanged();

    private void RefreshValidation()
    {
        StatusMessage = string.Join(" ", CandidateRegistrationValidator.Validate(CreateRequest()).Select(error => error.Message));
        UpdateSaveCommand();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private sealed class AsyncCommand(Func<Task> execute, Func<bool> canExecute) : ICommand
    {
        private bool _executing;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => !_executing && canExecute();

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            _executing = true;
            NotifyCanExecuteChanged();
            try
            {
                await execute();
            }
            finally
            {
                _executing = false;
                NotifyCanExecuteChanged();
            }
        }

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
