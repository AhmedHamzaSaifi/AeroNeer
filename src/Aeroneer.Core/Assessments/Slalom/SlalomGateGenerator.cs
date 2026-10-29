namespace Aeroneer.Core.Assessments.Slalom;

/// <summary>Generates a reproducible sequence of achievable Slalom gates.</summary>
public sealed class SlalomGateGenerator
{
    private readonly Random _random;

    /// <summary>Creates a generator with a deterministic seed.</summary>
    public SlalomGateGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>Creates gates with openings and centers guaranteed to fit within the flight area.</summary>
    public IReadOnlyList<SlalomGate> Generate(int gateCount)
    {
        if (gateCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(gateCount));
        }

        var gates = new SlalomGate[gateCount];
        var previousCenter = 0d;
        for (var index = 0; index < gateCount; index++)
        {
            var gapWidth = SlalomRules.MinimumSafeGapWidth
                + _random.NextDouble() * (SlalomRules.MaximumGapWidth - SlalomRules.MinimumSafeGapWidth);
            var safeCenterLimit = SlalomRules.MaximumLateralPosition - gapWidth / 2d;
            var maximumChange = Math.Min(0.55d, safeCenterLimit * 2d);
            var minimumCenter = Math.Max(-safeCenterLimit, previousCenter - maximumChange);
            var maximumCenter = Math.Min(safeCenterLimit, previousCenter + maximumChange);
            var center = minimumCenter + _random.NextDouble() * (maximumCenter - minimumCenter);

            gates[index] = new SlalomGate(index + 1, center, gapWidth, 1d + index * 0.15d);
            previousCenter = center;
        }

        return gates;
    }
}
