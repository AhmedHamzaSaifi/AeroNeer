namespace Aeroneer.Core.Models;

/// <summary>Persistence port for local candidate profiles.</summary>
public interface ICandidateRepository
{
    /// <summary>Creates a new candidate or updates the existing profile with the same identifier.</summary>
    Task<CandidateProfile> SaveAsync(CandidateProfile candidate, CancellationToken cancellationToken);

    /// <summary>Returns candidates ordered for a local returning-candidate selector.</summary>
    Task<IReadOnlyList<CandidateProfile>> GetAllAsync(CancellationToken cancellationToken);
}
