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
                Name = "Green Valley Recycling",
                Address = "123 Eco Lane",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 0.5,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "City Center Drop-off",
                Address = "456 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 1.2,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Northside Recycle Hub",
                Address = "789 North Ave",
                City = "Springfield",
                State = "IL",
                ZipCode = "62702",
                DistanceInMiles = 3.5,
                PhoneNumber = "555-0103"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrEmpty(zipCode))
        {
            return Enumerable.Empty<RecyclingFacility>();
        }

        return _facilities.Where(f => f.ZipCode == zipCode);
    }
}