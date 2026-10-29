namespace Aeroneer.Core.Models;

/// <summary>Candidate registration input; required fields are validated by CandidateRegistrationValidator.</summary>
public sealed record CandidateRegistrationRequest(
    string? CompanyOrInstitute,
    string? FirstName,
    string? LastName,
    string? Address,
    string? ZipCode,
    string? Place,
    string? Country,
    DateOnly? BirthDate,
    string? Gender,
    string? Telephone,
    string? Email);
