using System.Data;
using Aeroneer.Core.Assessments;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Aeroneer.Infrastructure.Data;

/// <summary>Persists lifecycle transitions and safely claims limited scored attempts.</summary>
public sealed class EfAssessmentLifecycleStore(IDbContextFactory<AppDbContext> contextFactory) : IAssessmentLifecycleStore
{
    private const int MaximumScoredAttempts = 3;

    public async Task<AssessmentAttempt> StartPracticeAsync(
        Guid candidateId,
        AssessmentModule module,
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = new AssessmentAttemptEntity
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            Module = module,
            Mode = AssessmentMode.Practice,
            AttemptNumber = 0,
            Status = AssessmentStatus.InProgress,
            StartedAtUtc = startedAtUtc.ToUniversalTime()
        };
        context.AssessmentAttempts.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return ToAttempt(entity);
    }

    public async Task<AttemptStartResult> TryStartScoredAttemptAsync(
        Guid candidateId,
        AssessmentModule module,
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var hasCompletedPractice = await context.CandidateModuleProgresses
            .AnyAsync(progress => progress.CandidateId == candidateId
                && progress.Module == module
                && progress.PracticeCompleted, cancellationToken);
        if (!hasCompletedPractice)
        {
            return AttemptStartResult.Rejected("Complete a practice trial before starting a scored assessment.");
        }

        var scoredAttempts = await context.AssessmentAttempts
            .Where(attempt => attempt.CandidateId == candidateId
                && attempt.Module == module
                && attempt.Mode == AssessmentMode.Assessment)
            .Select(attempt => attempt.AttemptNumber)
            .ToArrayAsync(cancellationToken);
        if (scoredAttempts.Length >= MaximumScoredAttempts)
        {
            return AttemptStartResult.Rejected("The maximum of three scored attempts for this module has been used.");
        }

        var nextAttemptNumber = scoredAttempts.Length + 1;
        var entity = new AssessmentAttemptEntity
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            Module = module,
            Mode = AssessmentMode.Assessment,
            AttemptNumber = nextAttemptNumber,
            Status = AssessmentStatus.InProgress,
            StartedAtUtc = startedAtUtc.ToUniversalTime()
        };
        context.AssessmentAttempts.Add(entity);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AttemptStartResult.Started(ToAttempt(entity));
        }
        catch (Exception exception) when (exception is DbUpdateException
            or PostgresException { SqlState: PostgresErrorCodes.SerializationFailure })
        {
            await transaction.RollbackAsync(cancellationToken);
            return AttemptStartResult.Rejected("This attempt could not be claimed safely. Refresh progress and try again.");
        }
    }

    public async Task CompletePracticeAsync(
        Guid attemptId,
        DateTimeOffset finishedAtUtc,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var attempt = await context.AssessmentAttempts.SingleOrDefaultAsync(item => item.Id == attemptId, cancellationToken)
            ?? throw new InvalidOperationException("The practice attempt was not found.");
        EnsureInProgress(attempt);
        if (attempt.Mode != AssessmentMode.Practice)
        {
            throw new InvalidOperationException("Only practice attempts can complete the practice prerequisite.");
        }

        var finishedUtc = finishedAtUtc.ToUniversalTime();
        attempt.Status = AssessmentStatus.Completed;
        attempt.FinishedAtUtc = finishedUtc;
        attempt.DurationMilliseconds = (long)duration.TotalMilliseconds;
        var progress = await context.CandidateModuleProgresses.SingleOrDefaultAsync(item =>
            item.CandidateId == attempt.CandidateId && item.Module == attempt.Module, cancellationToken);
        if (progress is null)
        {
            context.CandidateModuleProgresses.Add(new CandidateModuleProgressEntity
            {
                CandidateId = attempt.CandidateId,
                Module = attempt.Module,
                PracticeCompleted = true,
                PracticeCompletedAtUtc = finishedUtc
            });
        }
        else
        {
            progress.PracticeCompleted = true;
            progress.PracticeCompletedAtUtc = finishedUtc;
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task FinishAttemptAsync(
        Guid attemptId,
        AssessmentStatus terminalStatus,
        DateTimeOffset finishedAtUtc,
        TimeSpan duration,
        double? score,
        CancellationToken cancellationToken)
    {
        if (terminalStatus is not (AssessmentStatus.Completed or AssessmentStatus.Abandoned or AssessmentStatus.Invalid))
        {
            throw new ArgumentOutOfRangeException(nameof(terminalStatus), "An attempt must transition to a terminal status.");
        }

        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var attempt = await context.AssessmentAttempts.SingleOrDefaultAsync(item => item.Id == attemptId, cancellationToken)
            ?? throw new InvalidOperationException("The assessment attempt was not found.");
        EnsureInProgress(attempt);
        attempt.Status = terminalStatus;
        attempt.FinishedAtUtc = finishedAtUtc.ToUniversalTime();
        attempt.DurationMilliseconds = (long)duration.TotalMilliseconds;
        attempt.Score = terminalStatus == AssessmentStatus.Completed && attempt.Mode == AssessmentMode.Assessment && score.HasValue
            ? Math.Clamp(score.Value, 0d, 100d)
            : null;
        await context.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureInProgress(AssessmentAttemptEntity attempt)
    {
        if (attempt.Status != AssessmentStatus.InProgress)
        {
            throw new InvalidOperationException("Only an in-progress attempt can be finished.");
        }
    }

    private static AssessmentAttempt ToAttempt(AssessmentAttemptEntity entity) => new(
        entity.Id,
        entity.CandidateId,
        entity.Module,
        entity.Mode,
        entity.AttemptNumber,
        entity.Status,
        entity.StartedAtUtc,
        entity.FinishedAtUtc,
        entity.DurationMilliseconds is null ? null : TimeSpan.FromMilliseconds(entity.DurationMilliseconds.Value),
        entity.Score);
}
