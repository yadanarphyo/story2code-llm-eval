using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly List<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService()
    {
        _facilities =
        [
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
                Name = "Northside Eco Hub",
                Address = "789 Pine Rd",
                City = "Springfield",
                State = "IL",
                ZipCode = "62702",
                DistanceInMiles = 2.8,
                PhoneNumber = "555-0103"
            }
        ];
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Enumerable.Empty<RecyclingFacility>();
        }

        return _facilities.Where(f => f.ZipCode == zipCode).ToList();
    }
}