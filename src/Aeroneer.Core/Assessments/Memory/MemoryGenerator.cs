namespace Aeroneer.Core.Assessments.Memory;

/// <summary>Generates reproducible cockpit values and hidden-field schedules.</summary>
public sealed class MemoryGenerator
{
    private const int MinimumAltitudeFeet = 1_000;
    private const int MaximumAltitudeFeet = 39_000;
    private const int MinimumSpeedKnots = 100;
    private const int MaximumSpeedKnots = 450;
    private const int MinimumRadioKilohertz = 118_000;
    private const int MaximumRadioKilohertz = 136_975;
    private const int RadioStepKilohertz = 25;

    private static readonly MemoryField[] AllFields =
    [
        MemoryField.Altitude,
        MemoryField.Speed,
        MemoryField.Heading,
        MemoryField.Radio
    ];

    private readonly Random _random;

    /// <summary>Creates a generator with the supplied deterministic seed.</summary>
    public MemoryGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>Generates values within the documented assessment ranges.</summary>
    public MemoryParameters GenerateParameters()
    {
        var altitudeIncrement = _random.Next(2) == 0 ? 100 : 500;
        var altitudeSteps = (MaximumAltitudeFeet - MinimumAltitudeFeet) / altitudeIncrement;
        var altitude = MinimumAltitudeFeet + _random.Next(altitudeSteps + 1) * altitudeIncrement;
        var speed = _random.Next(MinimumSpeedKnots, MaximumSpeedKnots + 1);
        var heading = _random.Next(0, 360);
        var radioStepCount = (MaximumRadioKilohertz - MinimumRadioKilohertz) / RadioStepKilohertz;
        var radio = MinimumRadioKilohertz + _random.Next(radioStepCount + 1) * RadioStepKilohertz;

        return new MemoryParameters(altitude, speed, heading, radio);
    }

    /// <summary>Creates the prescribed assessment difficulty schedule for a round.</summary>
    public MemoryRoundPlan CreateRoundPlan(int roundNumber)
    {
        if (roundNumber is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(roundNumber), "Assessment rounds range from 1 through 10.");
        }

        var hiddenCount = roundNumber switch
        {
            <= 3 => 1,
            <= 6 => 2,
            <= 8 => 3,
            _ => 4
        };
        var displaySeconds = roundNumber switch
        {
            <= 3 => 8,
            <= 6 => 7,
            <= 8 => 6,
            _ => 5
        };
        var shuffledFields = (MemoryField[])AllFields.Clone();

        for (var index = shuffledFields.Length - 1; index > 0; index--)
        {
            var swapIndex = _random.Next(index + 1);
            (shuffledFields[index], shuffledFields[swapIndex]) = (shuffledFields[swapIndex], shuffledFields[index]);
        }

        return new MemoryRoundPlan(
            roundNumber,
            displaySeconds,
            new HashSet<MemoryField>(shuffledFields.Take(hiddenCount)));
    }
}
