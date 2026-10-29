namespace Aeroneer.Core.Assessments.Memory;

/// <summary>Represents generated cockpit data using canonical numeric units.</summary>
/// <param name="AltitudeFeet">Altitude in feet.</param>
/// <param name="SpeedKnots">Speed in knots.</param>
/// <param name="HeadingDegrees">Heading from 0 through 359 degrees.</param>
/// <param name="RadioFrequencyKilohertz">Radio frequency in kHz.</param>
public sealed record MemoryParameters(
    int AltitudeFeet,
    int SpeedKnots,
    int HeadingDegrees,
    int RadioFrequencyKilohertz)
{
    /// <summary>Formats the heading as a three-digit value.</summary>
    public string FormattedHeading => HeadingDegrees.ToString("D3", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Formats the radio frequency in MHz with three decimal places.</summary>
    public string FormattedRadioFrequency =>
        (RadioFrequencyKilohertz / 1000m).ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Gets the value for a requested field as an integer in its canonical unit.</summary>
    public int GetValue(MemoryField field) => field switch
    {
        MemoryField.Altitude => AltitudeFeet,
        MemoryField.Speed => SpeedKnots,
        MemoryField.Heading => HeadingDegrees,
        MemoryField.Radio => RadioFrequencyKilohertz,
        _ => throw new ArgumentOutOfRangeException(nameof(field))
    };
}
