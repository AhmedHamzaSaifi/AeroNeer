namespace Aeroneer.Core.Assessments.Slalom;

/// <summary>Summary of one 90-second Slalom run.</summary>
public sealed record SlalomRunResult(
    int RunNumber,
    double Score,
    int GatesPassed,
    int GatesMissed,
    int GatesCollided,
    double AverageCenterDeviation,
    IReadOnlyList<SlalomGateResult> GateResults);

/// <summary>Combined result of the three runs within one scored Slalom attempt.</summary>
public sealed record SlalomAssessmentResult(
    double Score,
    IReadOnlyList<SlalomRunResult> Runs);
