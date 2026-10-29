using System.ComponentModel.DataAnnotations;
using Aeroneer.Core.Models;

namespace Aeroneer.Core.Validation;

/// <summary>Validates required candidate registration fields and optional contact details.</summary>
public static class CandidateRegistrationValidator
{
    /// <summary>Returns field-specific errors; first name, last name, country, birth date, and email are required.</summary>
    public static IReadOnlyList<FieldValidationError> Validate(CandidateRegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var errors = new List<FieldValidationError>();
        Require(request.FirstName, "FirstName", "First name is required.");
        Require(request.LastName, "LastName", "Last name is required.");
        Require(request.Country, "Country", "Country is required.");
        if (request.BirthDate is null)
        {
            errors.Add(new FieldValidationError("BirthDate", "Birth date is required."));
        }
        else if (request.BirthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add(new FieldValidationError("BirthDate", "Birth date cannot be in the future."));
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add(new FieldValidationError("Email", "E-mail is required."));
        }
        else if (!new EmailAddressAttribute().IsValid(request.Email.Trim()))
        {
            errors.Add(new FieldValidationError("Email", "Enter a valid e-mail address."));
        }

        return errors;

        void Require(string? value, string field, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new FieldValidationError(field, message));
            }
        }
    }
}
