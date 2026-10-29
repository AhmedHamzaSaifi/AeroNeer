namespace Aeroneer.Core.Assessments;

/// <summary>Describes whether an attempt was durably created.</summary>
public sealed record AttemptStartResult(
    bool Succeeded,
    AssessmentAttempt? Attempt,
    string? FailureReason)
{
    /// <summary>Creates a successful start result.</summary>
    public static AttemptStartResult Started(AssessmentAttempt attempt) => new(true, attempt, null);

    /// <summary>Creates a rejected start result.</summary>
    public static AttemptStartResult Rejected(string reason) => new(false, null, reason);
}
