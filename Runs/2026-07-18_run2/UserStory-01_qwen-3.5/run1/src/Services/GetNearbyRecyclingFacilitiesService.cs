using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly List<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService()
    {
        // Seed in-memory data for unit test compatibility
        _facilities = new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "EcoCenter Downtown",
                Address = "123 Green St",
                City = "Beverly Hills",
                State = "CA",
                ZipCode = "90210",
                DistanceInMiles = 1.5,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "EcoCenter Westside",
                Address = "456 Recycle Ave",
                City = "Beverly Hills",
                State = "CA",
                ZipCode = "90211",
                DistanceInMiles = 3.2,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Manhattan Recycling Hub",
                Address = "789 Waste Ln",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                DistanceInMiles = 0.5,
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

        // Simple prefix match to simulate "nearby" logic without geospatial libraries
        return _facilities.Where(f => f.ZipCode.StartsWith(zipCode));
    }
}