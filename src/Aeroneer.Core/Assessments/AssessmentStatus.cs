namespace Aeroneer.Core.Assessments;

/// <summary>Represents the persisted lifecycle state of an attempt.</summary>
public enum AssessmentStatus
{
    InProgress,
    Completed,
    Abandoned,
    Invalid
}
