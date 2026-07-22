using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly List<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService()
    {
        _facilities = new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "Downtown Recycling Center",
                Address = "123 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 1.2,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Westside Materials Recovery",
                Address = "456 Oak Ave",
                City = "Springfield",
                State = "IL",
                ZipCode = "62704",
                DistanceInMiles = 3.5,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Northside Transfer Station",
                Address = "789 Pine Rd",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 2.8,
                PhoneNumber = "555-0103"
            },
            new RecyclingFacility
            {
                FacilityId = 4,
                Name = "Metro Waste Solutions",
                Address = "321 Elm St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62702",
                DistanceInMiles = 5.0,
                PhoneNumber = "555-0104"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        return _facilities
            .Where(f => f.ZipCode.Equals(zipCode, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.DistanceInMiles)
            .ToList();
    }
}