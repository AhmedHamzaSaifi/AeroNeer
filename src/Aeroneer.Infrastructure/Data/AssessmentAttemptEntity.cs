using Aeroneer.Core.Assessments;

namespace Aeroneer.Infrastructure.Data;

/// <summary>EF persistence representation of one practice or scored attempt.</summary>
public sealed class AssessmentAttemptEntity
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public CandidateEntity Candidate { get; set; } = null!;
    public AssessmentModule Module { get; set; }
    public AssessmentMode Mode { get; set; }
    public int AttemptNumber { get; set; }
    public AssessmentStatus Status { get; set; }
    public DateTimeOffset StartedAtUtc { get; set; }
    public DateTimeOffset? FinishedAtUtc { get; set; }
    public long? DurationMilliseconds { get; set; }
    public double? Score { get; set; }
}
