using Aeroneer.Core.Assessments;

namespace Aeroneer.Infrastructure.Data;

/// <summary>EF persistence representation of a candidate's progress in one module.</summary>
public sealed class CandidateModuleProgressEntity
{
    public Guid CandidateId { get; set; }
    public CandidateEntity Candidate { get; set; } = null!;
    public AssessmentModule Module { get; set; }
    public bool PracticeCompleted { get; set; }
    public DateTimeOffset? PracticeCompletedAtUtc { get; set; }
}
