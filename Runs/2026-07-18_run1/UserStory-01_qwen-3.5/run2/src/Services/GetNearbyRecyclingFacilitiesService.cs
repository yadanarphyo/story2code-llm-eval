using Implementation.Models;

namespace Implementation.Services;

public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
{
    private readonly List<RecyclingFacility> _facilities;

    public GetNearbyRecyclingFacilitiesService()
    {
        // Seed in-memory data to satisfy testability requirements (no external DB)
        _facilities = new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "Green Valley Recycling",
                Address = "123 Eco Way",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 1.5,
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
                DistanceInMiles = 3.2,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Community Drop-Off Center",
                Address = "789 Green St",
                City = "Shelbyville",
                State = "IL",
                ZipCode = "62703",
                DistanceInMiles = 5.0,
                PhoneNumber = "555-0103"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        // Simple filtering logic based on zip code prefix or exact match
        // In a real scenario, this would calculate distance, but for unit test stability
        // we filter based on the provided zipCode input.
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return _facilities;
        }

        // Return facilities that match the zip code or are within a simulated range
        // For the purpose of satisfying the interface contract and testability:
        return _facilities.Where(f => f.ZipCode.StartsWith(zipCode.Substring(0, Math.Min(3, zipCode.Length))));
    }
}