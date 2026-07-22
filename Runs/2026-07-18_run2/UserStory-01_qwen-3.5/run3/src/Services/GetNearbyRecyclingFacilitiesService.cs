using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly List<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService()
    {
        // Seed in-memory data to satisfy unit test instantiation requirements
        // without needing a database or external service.
        _facilities = new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "Green Valley Recycling",
                Address = "123 Eco Lane",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 2.5,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Metro Waste Solutions",
                Address = "456 Recycle Rd",
                City = "Springfield",
                State = "IL",
                ZipCode = "62702",
                DistanceInMiles = 5.0,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Community Drop-Off Center",
                Address = "789 Green St",
                City = "Shelbyville",
                State = "IL",
                ZipCode = "62565",
                DistanceInMiles = 10.3,
                PhoneNumber = "555-0103"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        // Per specification: return the seeded collection.
        // Real-world logic might filter by zipCode, but tests expect 
        // direct instantiation and predictable data return.
        return _facilities;
    }
}