namespace Aeroneer.Core.Assessments;

/// <summary>Defines persistence operations needed by the shared attempt lifecycle.</summary>
public interface IAssessmentLifecycleStore
{
    /// <summary>Creates a practice attempt; practice attempts do not have a start limit.</summary>
    Task<AssessmentAttempt> StartPracticeAsync(Guid candidateId, AssessmentModule module, DateTimeOffset startedAtUtc, CancellationToken cancellationToken);

    /// <summary>Atomically checks prerequisites/limit and persists a scored attempt as InProgress.</summary>
    /// <remarks>The implementation must claim the attempt inside a database transaction so concurrent starts cannot exceed the limit.</remarks>
    Task<AttemptStartResult> TryStartScoredAttemptAsync(Guid candidateId, AssessmentModule module, DateTimeOffset startedAtUtc, CancellationToken cancellationToken);

    /// <summary>Marks a practice trial complete and records its prerequisite for the candidate/module.</summary>
    Task CompletePracticeAsync(Guid attemptId, DateTimeOffset finishedAtUtc, TimeSpan duration, CancellationToken cancellationToken);

    /// <summary>Transitions a running attempt to a terminal state and optionally records its score.</summary>
    Task FinishAttemptAsync(Guid attemptId, AssessmentStatus terminalStatus, DateTimeOffset finishedAtUtc, TimeSpan duration, double? score, CancellationToken cancellationToken);
}
