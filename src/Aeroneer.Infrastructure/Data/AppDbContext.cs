using Microsoft.EntityFrameworkCore;

namespace Aeroneer.Infrastructure.Data;

/// <summary>Short-lived EF Core context for local PostgreSQL persistence.</summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CandidateEntity> Candidates => Set<CandidateEntity>();
    public DbSet<AssessmentAttemptEntity> AssessmentAttempts => Set<AssessmentAttemptEntity>();
    public DbSet<CandidateModuleProgressEntity> CandidateModuleProgresses => Set<CandidateModuleProgressEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var candidate = modelBuilder.Entity<CandidateEntity>();
        candidate.ToTable("Candidates");
        candidate.HasKey(entity => entity.Id);
        candidate.Property(entity => entity.CompanyOrInstitute).HasMaxLength(200);
        candidate.Property(entity => entity.FirstName).HasMaxLength(100).IsRequired();
        candidate.Property(entity => entity.LastName).HasMaxLength(100).IsRequired();
        candidate.Property(entity => entity.Address).HasMaxLength(300);
        candidate.Property(entity => entity.ZipCode).HasMaxLength(32);
        candidate.Property(entity => entity.Place).HasMaxLength(120);
        candidate.Property(entity => entity.Country).HasMaxLength(2).IsRequired();
        candidate.Property(entity => entity.Gender).HasMaxLength(64);
        candidate.Property(entity => entity.Telephone).HasMaxLength(64);
        candidate.Property(entity => entity.Email).HasMaxLength(254).IsRequired();
        candidate.HasIndex(entity => entity.LastName);
        candidate.HasIndex(entity => entity.Email);
        candidate.HasIndex(entity => new { entity.LastName, entity.FirstName });

        var attempt = modelBuilder.Entity<AssessmentAttemptEntity>();
        attempt.ToTable("AssessmentAttempts");
        attempt.HasKey(entity => entity.Id);
        attempt.Property(entity => entity.Module).HasConversion<string>().HasMaxLength(32);
        attempt.Property(entity => entity.Mode).HasConversion<string>().HasMaxLength(16);
        attempt.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(16);
        attempt.Property(entity => entity.StartedAtUtc).IsRequired();
        attempt.Property(entity => entity.DurationMilliseconds);
        attempt.Property(entity => entity.Score);
        attempt.HasOne(entity => entity.Candidate)
            .WithMany()
            .HasForeignKey(entity => entity.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
        attempt.HasIndex(entity => new { entity.CandidateId, entity.Module, entity.Mode, entity.AttemptNumber })
            .IsUnique()
            .HasFilter("\"Mode\" = 'Assessment'");
        attempt.HasIndex(entity => new { entity.CandidateId, entity.Module, entity.Mode, entity.StartedAtUtc });

        var progress = modelBuilder.Entity<CandidateModuleProgressEntity>();
        progress.ToTable("CandidateModuleProgress");
        progress.HasKey(entity => new { entity.CandidateId, entity.Module });
        progress.Property(entity => entity.Module).HasConversion<string>().HasMaxLength(32);
        progress.HasOne(entity => entity.Candidate)
            .WithMany()
            .HasForeignKey(entity => entity.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
