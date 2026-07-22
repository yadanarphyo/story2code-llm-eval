using Implementation.Models;

namespace Implementation.Services
{
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
                    Name = "Green Earth Recycling",
                    Address = "123 Main St",
                    City = "Springfield",
                    State = "IL",
                    ZipCode = "62701",
                    DistanceInMiles = 0.8,
                    PhoneNumber = "555-1234"
                },
                new RecyclingFacility
                {
                    FacilityId = 2,
                    Name = "Metro Recycling Center",
                    Address = "456 Oak Ave",
                    City = "Springfield",
                    State = "IL",
                    ZipCode = "62701",
                    DistanceInMiles = 1.5,
                    PhoneNumber = "555-5678"
                },
                new RecyclingFacility
                {
                    FacilityId = 3,
                    Name = "Suburban Waste Solutions",
                    Address = "789 Pine Rd",
                    City = "Shelbyville",
                    State = "IL",
                    ZipCode = "62702",
                    DistanceInMiles = 3.2,
                    PhoneNumber = "555-9012"
                }
            };
        }

        public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
        {
            return _facilities
                .Where(f => f.ZipCode == zipCode)
                .OrderBy(f => f.FacilityId)
                .ToList();
        }
    }
}