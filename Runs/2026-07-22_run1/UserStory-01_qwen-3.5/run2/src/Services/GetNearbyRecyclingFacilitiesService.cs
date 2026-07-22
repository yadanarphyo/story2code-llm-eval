using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly IEnumerable<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)
    {
        _facilities = facilities;
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            throw new ArgumentException("zipCode is required.", nameof(zipCode));
        }

        // Filter based on ZipCode match as per specification logic
        // Returns empty collection if no matches found, never null
        return _facilities.Where(f => f.ZipCode == zipCode).ToList();
    }
}