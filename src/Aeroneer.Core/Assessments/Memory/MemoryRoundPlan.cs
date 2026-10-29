namespace Aeroneer.Core.Assessments.Memory;

/// <summary>Defines the display and recall requirements for a memory round.</summary>
/// <param name="RoundNumber">One-based round number.</param>
/// <param name="DisplaySeconds">Seconds for which the cockpit values are visible.</param>
/// <param name="HiddenFields">Fields to recall after the display is hidden.</param>
public sealed record MemoryRoundPlan(
    int RoundNumber,
    int DisplaySeconds,
    IReadOnlySet<MemoryField> HiddenFields)
{
    /// <summary>Gets the number of fields the candidate must recall.</summary>
    public int RequestedFieldCount => HiddenFields.Count;

    /// <summary>Gets the scoring weight for this round, based on recall difficulty.</summary>
    public int DifficultyWeight => Math.Max(1, RequestedFieldCount);
}
