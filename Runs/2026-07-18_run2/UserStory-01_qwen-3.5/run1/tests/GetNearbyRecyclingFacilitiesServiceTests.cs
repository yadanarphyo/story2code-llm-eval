using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-01 (GetNearbyRecyclingFacilities). These are compiled directly
// against the generated Implementation project (see Prompts/rules-file for the naming
// contract that makes this possible for every model/run).
public class GetNearbyRecyclingFacilitiesServiceTests
{
    private static IGetNearbyRecyclingFacilitiesService CreateService()
    {
        return new GetNearbyRecyclingFacilitiesService();
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithValidZipCode_ReturnsNonNullResult()
    {
        var service = CreateService();

        var result = service.GetNearbyRecyclingFacilities("62701");

        Assert.NotNull(result);
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithValidZipCode_ReturnsFacilitiesWithRequiredFields()
    {
        var service = CreateService();

        var result = service.GetNearbyRecyclingFacilities("62701");

        Assert.NotNull(result);
        foreach (RecyclingFacility facility in result)
        {
            Assert.True(facility.FacilityId > 0);
            Assert.False(string.IsNullOrWhiteSpace(facility.Name));
            Assert.False(string.IsNullOrWhiteSpace(facility.Address));
            Assert.False(string.IsNullOrWhiteSpace(facility.City));
            Assert.False(string.IsNullOrWhiteSpace(facility.State));
            Assert.False(string.IsNullOrWhiteSpace(facility.ZipCode));
            Assert.True(facility.DistanceInMiles >= 0);
            Assert.False(string.IsNullOrWhiteSpace(facility.PhoneNumber));
        }
    }
}
