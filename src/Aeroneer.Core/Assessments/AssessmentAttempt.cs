namespace Aeroneer.Core.Assessments;

/// <summary>Describes a persisted attempt without depending on a UI or database framework.</summary>
public sealed record AssessmentAttempt(
    Guid Id,
    Guid CandidateId,
    AssessmentModule Module,
    AssessmentMode Mode,
    int AttemptNumber,
    AssessmentStatus Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? FinishedAtUtc,
    TimeSpan? Duration,
    double? Score);
