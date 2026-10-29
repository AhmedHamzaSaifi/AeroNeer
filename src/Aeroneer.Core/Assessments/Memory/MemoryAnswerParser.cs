using System.Globalization;

namespace Aeroneer.Core.Assessments.Memory;

/// <summary>Parses and validates a candidate's answer for one memory field.</summary>
public static class MemoryAnswerParser
{
    /// <summary>Attempts to parse an answer to a canonical integer value.</summary>
    /// <remarks>Radio values are returned in kHz; a matching field suffix is optional.</remarks>
    public static bool TryParse(MemoryField field, string? input, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var text = input.Trim();
        var suffix = char.ToLowerInvariant(text[^1]);
        var expectedSuffix = field switch
        {
            MemoryField.Altitude => 'a',
            MemoryField.Speed => 's',
            MemoryField.Heading => 'h',
            MemoryField.Radio => 'r',
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        if (char.IsLetter(suffix))
        {
            if (suffix != expectedSuffix)
            {
                return false;
            }

            text = text[..^1].TrimEnd();
        }

        if (field == MemoryField.Radio)
        {
            if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var megahertz))
            {
                return false;
            }

            var kilohertz = megahertz * 1_000m;
            if (kilohertz != decimal.Truncate(kilohertz) || kilohertz is < 118_000m or > 136_975m)
            {
                return false;
            }

            value = (int)kilohertz;
            return (value - 118_000) % 25 == 0;
        }

        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var numericValue))
        {
            return false;
        }

        var isValid = field switch
        {
            MemoryField.Altitude => numericValue is >= 1_000 and <= 39_000,
            MemoryField.Speed => numericValue is >= 100 and <= 450,
            MemoryField.Heading => numericValue is >= 0 and <= 359,
            _ => false
        };
        if (isValid)
        {
            value = numericValue;
        }

        return isValid;
    }
}
