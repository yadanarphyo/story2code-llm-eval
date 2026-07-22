using Implementation.Models;

namespace Implementation.Services
{
    public interface IGetNearbyRecyclingFacilitiesService
    {
        IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);
    }
}