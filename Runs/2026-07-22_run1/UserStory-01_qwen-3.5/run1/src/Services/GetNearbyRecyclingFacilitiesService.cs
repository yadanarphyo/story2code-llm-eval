using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly IEnumerable<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)
    {
        _facilities = facilities ?? Enumerable.Empty<RecyclingFacility>();
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            throw new ArgumentException("zipCode is required.", nameof(zipCode));
        }

        // Filter the injected candidate dataset by the provided zip code.
        // This ensures that if no matching data exists, an empty collection is returned.
        var result = _facilities.Where(f => f.ZipCode == zipCode);

        return result.ToList();
    }
}