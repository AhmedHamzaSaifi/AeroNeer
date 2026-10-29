namespace Aeroneer.Infrastructure.Data;

/// <summary>EF persistence representation of one local candidate.</summary>
public sealed class CandidateEntity
{
    public Guid Id { get; set; }
    public string? CompanyOrInstitute { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? Place { get; set; }
    public required string Country { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Telephone { get; set; }
    public required string Email { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
