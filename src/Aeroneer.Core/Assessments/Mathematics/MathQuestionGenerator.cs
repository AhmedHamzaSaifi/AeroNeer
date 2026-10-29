using System.Globalization;

namespace Aeroneer.Core.Assessments.Mathematics;

/// <summary>Creates reproducible applied arithmetic questions without ambiguous rounding.</summary>
public sealed class MathQuestionGenerator
{
    private static readonly TimeSpan DefaultResponseTimeout = TimeSpan.FromSeconds(30);
    private readonly Random _random;

    /// <summary>Creates a generator with a deterministic seed.</summary>
    public MathQuestionGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>Generates a question from the supported aviation arithmetic categories.</summary>
    public MathQuestion Generate()
    {
        var type = (MathQuestionType)_random.Next(Enum.GetValues<MathQuestionType>().Length);
        return type switch
        {
            MathQuestionType.Arithmetic => CreateArithmetic(),
            MathQuestionType.SpeedDistanceTime => CreateSpeedDistanceTime(),
            MathQuestionType.Fuel => CreateFuel(),
            MathQuestionType.UnitConversion => CreateConversion(),
            MathQuestionType.Ratio => CreateRatio(),
            _ => throw new InvalidOperationException("Unsupported mathematics question type.")
        };
    }

    /// <summary>Generates a fixed-size question set for practice or assessment.</summary>
    public IReadOnlyList<MathQuestion> GenerateSet(int questionCount)
    {
        if (questionCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(questionCount));
        }

        return Enumerable.Range(0, questionCount).Select(_ => Generate()).ToArray();
    }

    private MathQuestion CreateArithmetic()
    {
        var left = _random.Next(18, 251);
        var right = _random.Next(4, 101);
        var operation = _random.Next(4);
        return operation switch
        {
            0 => Create(MathQuestionType.Arithmetic, $"What is {left} + {right}?", left + right),
            1 => Create(MathQuestionType.Arithmetic, $"What is {left + right} - {right}?", left),
            2 => Create(MathQuestionType.Arithmetic, $"What is {left} x {right}?", left * right),
            _ => Create(MathQuestionType.Arithmetic, $"What is {left * right} / {right}?", left)
        };
    }

    private MathQuestion CreateSpeedDistanceTime()
    {
        var distanceKilometers = _random.Next(4, 61) * 5;
        var durationMinutes = _random.Next(2, 13) * 5;
        var speedKilometersPerHour = distanceKilometers * 60m / durationMinutes;
        var text = $"An aircraft covers {distanceKilometers} km in {durationMinutes} minutes. What is its average speed in km/h?";
        return Create(MathQuestionType.SpeedDistanceTime, text, speedKilometersPerHour);
    }

    private MathQuestion CreateFuel()
    {
        var rateLitersPerHour = _random.Next(20, 91) * 2;
        var durationHours = _random.Next(1, 9) / 4m;
        var fuelLiters = rateLitersPerHour * durationHours;
        var text = $"Fuel flow is {Format(rateLitersPerHour)} L/h for {Format(durationHours)} hours. How many liters are used?";
        return Create(MathQuestionType.Fuel, text, fuelLiters);
    }

    private MathQuestion CreateConversion()
    {
        var nauticalMiles = _random.Next(5, 151) * 2;
        var kilometers = nauticalMiles * 1.852m;
        var text = $"Convert {nauticalMiles} nautical miles to kilometers using 1 NM = 1.852 km.";
        return Create(MathQuestionType.UnitConversion, text, kilometers);
    }

    private MathQuestion CreateRatio()
    {
        var numerator = _random.Next(1, 8);
        var denominator = _random.Next(numerator + 1, 13);
        var total = denominator * _random.Next(10, 61);
        var part = total * numerator / denominator;
        var text = $"A route of {total} km is divided in the ratio {numerator}:{denominator - numerator}. How long is the first section?";
        return Create(MathQuestionType.Ratio, text, part);
    }

    private MathQuestion Create(MathQuestionType type, string text, decimal answer)
    {
        Span<byte> idBytes = stackalloc byte[16];
        _random.NextBytes(idBytes);
        return new MathQuestion(new Guid(idBytes), type, text, answer, 0m, DefaultResponseTimeout);
    }

    private static string Format(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
