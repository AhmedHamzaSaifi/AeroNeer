namespace Aeroneer.Core.Assessments.Slalom;

/// <summary>Shared Slalom duration and bounded gate-generation rules.</summary>
public static class SlalomRules
{
    /// <summary>Length of each scored run.</summary>
    public static readonly TimeSpan ScoredRunDuration = TimeSpan.FromSeconds(90);

    /// <summary>Number of scored runs in one assessment attempt.</summary>
    public const int ScoredRunsPerAttempt = 3;

    /// <summary>Minimum allowed normalized gate opening.</summary>
    public const double MinimumSafeGapWidth = 0.36d;

    /// <summary>Maximum allowed normalized gate opening.</summary>
    public const double MaximumGapWidth = 0.62d;

    /// <summary>Maximum magnitude of the aircraft's normalized lateral coordinate.</summary>
    public const double MaximumLateralPosition = 1d;

    /// <summary>Penalty percentage applied to a missed or collided gate outcome.</summary>
    public const double MissPenaltyPercent = 100d;
}
