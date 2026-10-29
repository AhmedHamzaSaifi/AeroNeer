using Aeroneer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Aeroneer.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
public sealed class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");
        var candidate = modelBuilder.Entity<CandidateEntity>();
        candidate.Property(entity => entity.Id).HasColumnType("uuid");
        candidate.Property(entity => entity.Address).HasMaxLength(300);
        candidate.Property(entity => entity.BirthDate).HasColumnType("date");
        candidate.Property(entity => entity.CompanyOrInstitute).HasMaxLength(200);
        candidate.Property(entity => entity.Country).HasMaxLength(2).IsRequired();
        candidate.Property(entity => entity.CreatedAtUtc).HasColumnType("timestamp with time zone");
        candidate.Property(entity => entity.Email).HasMaxLength(254).IsRequired();
        candidate.Property(entity => entity.FirstName).HasMaxLength(100).IsRequired();
        candidate.Property(entity => entity.Gender).HasMaxLength(64);
        candidate.Property(entity => entity.LastName).HasMaxLength(100).IsRequired();
        candidate.Property(entity => entity.Place).HasMaxLength(120);
        candidate.Property(entity => entity.Telephone).HasMaxLength(64);
        candidate.Property(entity => entity.UpdatedAtUtc).HasColumnType("timestamp with time zone");
        candidate.Property(entity => entity.ZipCode).HasMaxLength(32);
        candidate.HasKey(entity => entity.Id);
        candidate.HasIndex(entity => entity.Email);
        candidate.HasIndex(entity => entity.LastName);
        candidate.HasIndex(entity => new { entity.LastName, entity.FirstName });
        candidate.ToTable("Candidates");

        var attempt = modelBuilder.Entity<AssessmentAttemptEntity>();
        attempt.Property(entity => entity.Id).HasColumnType("uuid");
        attempt.Property(entity => entity.CandidateId).HasColumnType("uuid");
        attempt.Property(entity => entity.Module).HasConversion<string>().HasMaxLength(32);
        attempt.Property(entity => entity.Mode).HasConversion<string>().HasMaxLength(16);
        attempt.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(16);
        attempt.Property(entity => entity.StartedAtUtc).HasColumnType("timestamp with time zone");
        attempt.Property(entity => entity.FinishedAtUtc).HasColumnType("timestamp with time zone");
        attempt.Property(entity => entity.DurationMilliseconds).HasColumnType("bigint");
        attempt.Property(entity => entity.Score).HasColumnType("double precision");
        attempt.HasKey(entity => entity.Id);
        attempt.HasIndex(entity => new { entity.CandidateId, entity.Module, entity.Mode, entity.AttemptNumber })
            .IsUnique()
            .HasFilter("\"Mode\" = 'Assessment'");
        attempt.HasIndex(entity => new { entity.CandidateId, entity.Module, entity.Mode, entity.StartedAtUtc });
        attempt.HasOne(entity => entity.Candidate).WithMany().HasForeignKey(entity => entity.CandidateId).OnDelete(DeleteBehavior.Cascade);
        attempt.ToTable("AssessmentAttempts");

        var progress = modelBuilder.Entity<CandidateModuleProgressEntity>();
        progress.Property(entity => entity.CandidateId).HasColumnType("uuid");
        progress.Property(entity => entity.Module).HasConversion<string>().HasMaxLength(32);
        progress.Property(entity => entity.PracticeCompletedAtUtc).HasColumnType("timestamp with time zone");
        progress.HasKey(entity => new { entity.CandidateId, entity.Module });
        progress.HasOne(entity => entity.Candidate).WithMany().HasForeignKey(entity => entity.CandidateId).OnDelete(DeleteBehavior.Cascade);
        progress.ToTable("CandidateModuleProgress");
    }
}
