namespace Aeroneer.Core.Assessments.Orientation;

/// <summary>Generates internally consistent, offline instrument questions with deterministic seeds.</summary>
public sealed class OrientationQuestionGenerator
{
    private readonly Random _random;

    /// <summary>Creates a generator with a deterministic seed.</summary>
    public OrientationQuestionGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>Creates a question where the matching symbol combines compass, RBI, and horizon readings.</summary>
    public OrientationQuestion Generate()
    {
        var compassHeading = _random.Next(0, 360);
        var relativeBearing = _random.Next(0, 360);
        var pitch = _random.Next(-15, 16);
        var bankMagnitude = _random.Next(5, 46);
        var bank = _random.Next(2) == 0 ? -bankMagnitude : bankMagnitude;
        var absoluteBeaconBearing = Normalize(compassHeading + relativeBearing);
        var correctSymbol = new OrientationSymbol(absoluteBeaconBearing, pitch, bank);
        var options = new OrientationSymbol[4];
        var correctOption = _random.Next(1, 5);
        options[correctOption - 1] = correctSymbol;

        var distractorIndex = 0;
        while (distractorIndex < options.Length)
        {
            if (distractorIndex == correctOption - 1)
            {
                distractorIndex++;
                continue;
            }

            var distractor = distractorIndex % 3 switch
            {
                0 => correctSymbol with { BeaconBearingDegrees = Normalize(absoluteBeaconBearing + 90 + _random.Next(180)) },
                1 => correctSymbol with { PitchDegrees = pitch < 15 ? pitch + 1 : pitch - 1 },
                _ => correctSymbol with { BankDegrees = -bank }
            };
            if (!options.Take(distractorIndex).Contains(distractor))
            {
                options[distractorIndex] = distractor;
                distractorIndex++;
            }
        }

        var questionId = CreateDeterministicId();
        var explanation = $"Compass heading {compassHeading:D3} degrees plus relative bearing {relativeBearing:D3} degrees gives beacon bearing {absoluteBeaconBearing:D3}. The matching symbol also shows pitch {pitch} and bank {bank} degrees.";
        return new OrientationQuestion(
            questionId,
            compassHeading,
            relativeBearing,
            pitch,
            bank,
            absoluteBeaconBearing,
            options,
            correctOption,
            explanation);
    }

    /// <summary>Creates an ordered question set for the selected mode.</summary>
    public IReadOnlyList<OrientationQuestion> GenerateSet(int questionCount)
    {
        if (questionCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(questionCount));
        }

        return Enumerable.Range(0, questionCount).Select(_ => Generate()).ToArray();
    }

    private Guid CreateDeterministicId()
    {
        Span<byte> idBytes = stackalloc byte[16];
        _random.NextBytes(idBytes);
        return new Guid(idBytes);
    }

    private static int Normalize(int heading) => (heading % 360 + 360) % 360;
}
