using System.Globalization;

namespace Aeroneer.Core.Models;

/// <summary>Builds a stable offline country list from the runtime's installed globalization data.</summary>
public static class CountryOptions
{
    /// <summary>Returns distinct two-letter regions sorted by their localized English names.</summary>
    public static IReadOnlyList<CountryOption> GetAll()
    {
        var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => TryGetRegion(culture.Name))
            .Where(region => region is not null)
            .Select(region => new CountryOption(region!.TwoLetterISORegionName, region.EnglishName))
            .Where(option => option.Code.Length == 2)
            .DistinctBy(option => option.Code, StringComparer.OrdinalIgnoreCase)
            .OrderBy(option => option.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
        return regions;
    }

    private static RegionInfo? TryGetRegion(string cultureName)
    {
        try
        {
            return new RegionInfo(cultureName);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
