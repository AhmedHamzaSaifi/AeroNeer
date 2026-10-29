namespace Aeroneer.Core.Assessments.Slalom;

/// <summary>Calculates gate outcomes and bounded Slalom run/assessment scores.</summary>
public static class SlalomScoring
{
    private const double CenterDeviationPenaltyWeight = 0.65d;

    /// <summary>Evaluates a candidate crossing at the gate's vertical crossing line.</summary>
    public static SlalomGateResult EvaluateGate(SlalomGate gate, double aircraftPosition, bool gateCrossed)
    {
        ArgumentNullException.ThrowIfNull(gate);
        var position = Math.Clamp(aircraftPosition, -SlalomRules.MaximumLateralPosition, SlalomRules.MaximumLateralPosition);
        var deviation = Math.Abs(position - gate.Center);
        var gatePassed = gateCrossed && deviation <= gate.GapWidth / 2d;
        var collided = gateCrossed && deviation <= gate.GapWidth / 2d + 0.08d && !gatePassed;
        return new SlalomGateResult(gate.Index, position, deviation, gatePassed, collided);
    }

    /// <summary>Scores all scheduled gates, treating unrecorded gates as missed.</summary>
    public static SlalomRunResult ScoreRun(int runNumber, IReadOnlyList<SlalomGate> gates, IEnumerable<SlalomGateResult> outcomes)
    {
        ArgumentNullException.ThrowIfNull(gates);
        ArgumentNullException.ThrowIfNull(outcomes);
        if (runNumber is < 1 or > SlalomRules.ScoredRunsPerAttempt)
        {
            throw new ArgumentOutOfRangeException(nameof(runNumber));
        }

        if (gates.Count == 0)
        {
            throw new ArgumentException("A Slalom run must contain at least one gate.", nameof(gates));
        }

        var outcomeByGate = outcomes.ToDictionary(result => result.GateIndex);
        var completeOutcomes = gates.Select(gate => outcomeByGate.TryGetValue(gate.Index, out var result)
            ? result
            : new SlalomGateResult(gate.Index, null, null, false, false)).ToArray();
        var passed = completeOutcomes.Count(result => result.Passed);
        var collided = completeOutcomes.Count(result => result.Collided);
        var missed = gates.Count - passed - collided;
        var measuredDeviations = completeOutcomes
            .Where(result => result.CenterDeviation.HasValue)
            .Select(result => result.CenterDeviation!.Value)
            .ToArray();
        var meanDeviation = measuredDeviations.Length == 0 ? 0d : measuredDeviations.Average();
        var gateScores = completeOutcomes.Select((result, index) =>
        {
            if (!result.Passed)
            {
                return 0d;
            }

            var halfGap = gates[index].GapWidth / 2d;
            var normalizedDeviation = halfGap <= 0 || !result.CenterDeviation.HasValue
                ? 1d
                : Math.Clamp(result.CenterDeviation.Value / halfGap, 0d, 1d);
            return 100d * (1d - CenterDeviationPenaltyWeight * normalizedDeviation);
        });
        var score = gateScores.DefaultIfEmpty(0d).Average();

        return new SlalomRunResult(runNumber, Clamp(score), passed, missed, collided, meanDeviation, completeOutcomes);
    }

    /// <summary>Aggregates the three scored runs using their arithmetic mean.</summary>
    public static SlalomAssessmentResult ScoreAssessment(IEnumerable<SlalomRunResult> runs)
    {
        ArgumentNullException.ThrowIfNull(runs);
        var runResults = runs.OrderBy(result => result.RunNumber).ToArray();
        var score = runResults.Length == SlalomRules.ScoredRunsPerAttempt
            ? runResults.Average(result => Clamp(result.Score))
            : 0d;
        return new SlalomAssessmentResult(Clamp(score), runResults);
    }

    private static double Clamp(double score) => Math.Clamp(score, 0d, 100d);
}
