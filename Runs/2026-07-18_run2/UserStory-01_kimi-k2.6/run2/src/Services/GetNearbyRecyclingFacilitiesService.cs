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
                Name = "Springfield Recycling Center",
                Address = "123 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 1.2,
                PhoneNumber = "217-555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Capital City Recycling",
                Address = "456 Elm St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 2.5,
                PhoneNumber = "217-555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Rochester Recycling",
                Address = "789 Oak Ave",
                City = "Rochester",
                State = "IL",
                ZipCode = "62563",
                DistanceInMiles = 15.0,
                PhoneNumber = "217-555-0103"
            },
            new RecyclingFacility
            {
                FacilityId = 4,
                Name = "Chatham Facility",
                Address = "321 Pine Rd",
                City = "Chatham",
                State = "IL",
                ZipCode = "62629",
                DistanceInMiles = 8.7,
                PhoneNumber = "217-555-0104"
            },
            new RecyclingFacility
            {
                FacilityId = 5,
                Name = "Downtown Drop-off",
                Address = "555 Market St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62703",
                DistanceInMiles = 3.0,
                PhoneNumber = "217-555-0105"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Enumerable.Empty<RecyclingFacility>();
        }

        var matches = _facilities
            .Where(f => f.ZipCode == zipCode)
            .OrderBy(f => f.DistanceInMiles)
            .ToList();

        return matches.Any() ? matches : Enumerable.Empty<RecyclingFacility>();
    }
}