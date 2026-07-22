using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly List<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService()
    {
        // Seed in-memory data store to satisfy testability requirements
        // without external dependencies (DB/Network).
        _facilities = new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "Green Cycle Center",
                Address = "123 Eco Way",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 0.5,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Recycle Right Hub",
                Address = "456 Waste Not Blvd",
                City = "Springfield",
                State = "IL",
                ZipCode = "62702",
                DistanceInMiles = 1.2,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Community Drop-Off",
                Address = "789 Green St",
                City = "Shelbyville",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 5.0,
                PhoneNumber = "555-0103"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Enumerable.Empty<RecyclingFacility>();
        }

        // Filter based on exact zip code match
        return _facilities.Where(f => f.ZipCode.Equals(zipCode, StringComparison.OrdinalIgnoreCase));
    }
}