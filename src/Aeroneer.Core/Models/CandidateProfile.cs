namespace Aeroneer.Core.Models;

/// <summary>Candidate registration data; no authentication credentials are stored.</summary>
public sealed record CandidateProfile(
    Guid Id,
    string? CompanyOrInstitute,
    string FirstName,
    string LastName,
    string? Address,
    string? ZipCode,
    string? Place,
    string Country,
    DateOnly BirthDate,
    string? Gender,
    string? Telephone,
    string Email,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
