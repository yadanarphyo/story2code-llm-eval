using Implementation.Models;

namespace Implementation.Data;

public static class SeedData
{
    public static IEnumerable<RecyclingFacility> GetFacilities()
    {
        return new List<RecyclingFacility>
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
            }
        };
    }
}