using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-01 (GetNearbyRecyclingFacilities). Compiled directly against the
// generated Implementation project. Per Prompts/rules-file rule 3, the generated service must
// not hardcode any mock data of its own — it takes the candidate dataset as a constructor
// parameter, so every model/run is exercised against this same fixed fixture instead of
// whatever arbitrary data each model would otherwise invent.
public class GetNearbyRecyclingFacilitiesServiceTests
{
    private static List<RecyclingFacility> CreateFixture()
    {
        return new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "Green Recycling Center",
                Address = "123 Main Street",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 2.8,
                PhoneNumber = "(555) 123-4567"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Eco Waste Solutions",
                Address = "456 Oak Avenue",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 4.1,
                PhoneNumber = "(555) 987-6543"
            }
        };
    }

    private static IGetNearbyRecyclingFacilitiesService CreateService(IEnumerable<RecyclingFacility> facilities)
    {
        return new GetNearbyRecyclingFacilitiesService(facilities);
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithValidZipCode_ReturnsNonNullResult()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetNearbyRecyclingFacilities("62701");

        Assert.NotNull(result);
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_ReturnsOnlyFacilitiesFromSeededData()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seededIds = fixture.Select(f => f.FacilityId).ToHashSet();

        var result = service.GetNearbyRecyclingFacilities("62701");

        foreach (RecyclingFacility facility in result)
        {
            Assert.Contains(facility.FacilityId, seededIds);
        }
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WhenAllSeededDataMatchesZip_ReturnsAllOfIt()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetNearbyRecyclingFacilities("62701").ToList();

        Assert.Equal(fixture.Count, result.Count);
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetNearbyRecyclingFacilities("62701");

        foreach (RecyclingFacility facility in result)
        {
            var seeded = fixture.FirstOrDefault(f => f.FacilityId == facility.FacilityId);
            Assert.NotNull(seeded);
            Assert.Equal(seeded!.Name, facility.Name);
            Assert.Equal(seeded.Address, facility.Address);
            Assert.Equal(seeded.City, facility.City);
            Assert.Equal(seeded.State, facility.State);
            Assert.Equal(seeded.ZipCode, facility.ZipCode);
            Assert.Equal(seeded.DistanceInMiles, facility.DistanceInMiles);
            Assert.Equal(seeded.PhoneNumber, facility.PhoneNumber);
        }
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_ReturnsFacilitiesWithUniqueIds()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetNearbyRecyclingFacilities("62701").ToList();

        var ids = result.Select(f => f.FacilityId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithEmptyDataSet_ReturnsEmptyNotNull()
    {
        var service = CreateService(new List<RecyclingFacility>());

        var result = service.GetNearbyRecyclingFacilities("62701");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithZipCodeNotMatchingAnySeededFacility_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.GetNearbyRecyclingFacilities("99999"));

        Assert.Null(exception);
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithNullZipCode_ThrowsArgumentException()
    {
        var service = CreateService(CreateFixture());

        Assert.ThrowsAny<ArgumentException>(() => service.GetNearbyRecyclingFacilities(null!));
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithEmptyZipCode_ThrowsArgumentException()
    {
        var service = CreateService(CreateFixture());

        Assert.ThrowsAny<ArgumentException>(() => service.GetNearbyRecyclingFacilities(string.Empty));
    }

    [Fact]
    public void GetNearbyRecyclingFacilities_WithWhitespaceZipCode_ThrowsArgumentException()
    {
        var service = CreateService(CreateFixture());

        Assert.ThrowsAny<ArgumentException>(() => service.GetNearbyRecyclingFacilities("   "));
    }
}
