using Aeroneer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Aeroneer.Infrastructure.Data;

/// <summary>Persists candidate registration using short-lived factory-created contexts.</summary>
public sealed class EfCandidateRepository(IDbContextFactory<AppDbContext> contextFactory) : ICandidateRepository
{
    public async Task<CandidateProfile> SaveAsync(CandidateProfile candidate, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await context.Candidates.SingleOrDefaultAsync(item => item.Id == candidate.Id, cancellationToken);
        if (entity is null)
        {
            entity = new CandidateEntity { Id = candidate.Id, FirstName = candidate.FirstName, LastName = candidate.LastName, Country = candidate.Country, Email = candidate.Email };
            context.Candidates.Add(entity);
        }

        entity.CompanyOrInstitute = candidate.CompanyOrInstitute;
        entity.FirstName = candidate.FirstName;
        entity.LastName = candidate.LastName;
        entity.Address = candidate.Address;
        entity.ZipCode = candidate.ZipCode;
        entity.Place = candidate.Place;
        entity.Country = candidate.Country;
        entity.BirthDate = candidate.BirthDate;
        entity.Gender = candidate.Gender;
        entity.Telephone = candidate.Telephone;
        entity.Email = candidate.Email;
        entity.CreatedAtUtc = candidate.CreatedAtUtc;
        entity.UpdatedAtUtc = candidate.UpdatedAtUtc;
        await context.SaveChangesAsync(cancellationToken);
        return ToProfile(entity);
    }

    public async Task<IReadOnlyList<CandidateProfile>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var candidates = await context.Candidates
            .AsNoTracking()
            .OrderBy(entity => entity.LastName)
            .ThenBy(entity => entity.FirstName)
            .ToArrayAsync(cancellationToken);
        return candidates.Select(ToProfile).ToArray();
    }

    private static CandidateProfile ToProfile(CandidateEntity entity) => new(
        entity.Id,
        entity.CompanyOrInstitute,
        entity.FirstName,
        entity.LastName,
        entity.Address,
        entity.ZipCode,
        entity.Place,
        entity.Country,
        entity.BirthDate,
        entity.Gender,
        entity.Telephone,
        entity.Email,
        entity.CreatedAtUtc,
        entity.UpdatedAtUtc);
}
