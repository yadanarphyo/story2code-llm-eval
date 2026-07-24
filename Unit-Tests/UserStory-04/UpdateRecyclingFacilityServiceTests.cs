using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-04 (UpdateRecyclingFacility). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing recycling facilities) directly, so every model/run is
// exercised against this same fixed fixture rather than whatever arbitrary data each model
// would otherwise invent.
public class UpdateRecyclingFacilityServiceTests
{
    private static List<RecyclingFacility> CreateExistingFacilitiesFixture()
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
                PhoneNumber = null
            }
        };
    }

    private static IUpdateRecyclingFacilityService CreateService(IEnumerable<RecyclingFacility> existingFacilities)
    {
        return new UpdateRecyclingFacilityService(existingFacilities);
    }

    [Fact]
    public void UpdateRecyclingFacility_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        var result = service.UpdateRecyclingFacility(
            facilityId: 1,
            name: "Green Recycling Center",
            address: "789 New Street",
            city: "Springfield",
            state: "IL",
            zipCode: "62702",
            phoneNumber: "(555) 111-2222");

        Assert.NotNull(result);
    }

    [Fact]
    public void UpdateRecyclingFacility_WithValidInput_ReturnsMatchingFacilityId()
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        var result = service.UpdateRecyclingFacility(
            facilityId: 1,
            name: "Green Recycling Center",
            address: "789 New Street",
            city: "Springfield",
            state: "IL",
            zipCode: "62702",
            phoneNumber: "(555) 111-2222");

        Assert.Equal(1, result.FacilityId);
    }

    [Fact]
    public void UpdateRecyclingFacility_WithValidInput_UpdatesAllProvidedFields()
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        var result = service.UpdateRecyclingFacility(
            facilityId: 2,
            name: "Updated Eco Center",
            address: "999 Updated Ave",
            city: "Shelbyville",
            state: "IL",
            zipCode: "62703",
            phoneNumber: "(555) 999-8888");

        Assert.Equal("Updated Eco Center", result.Name);
        Assert.Equal("999 Updated Ave", result.Address);
        Assert.Equal("Shelbyville", result.City);
        Assert.Equal("IL", result.State);
        Assert.Equal("62703", result.ZipCode);
        Assert.Equal("(555) 999-8888", result.PhoneNumber);
    }

    [Fact]
    public void UpdateRecyclingFacility_WithNullPhoneNumber_DoesNotThrow()
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        var exception = Record.Exception(() => service.UpdateRecyclingFacility(
            facilityId: 1,
            name: "Green Recycling Center",
            address: "123 Main Street",
            city: "Springfield",
            state: "IL",
            zipCode: "62701",
            phoneNumber: null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateRecyclingFacility_WithNullPhoneNumber_ResultPhoneNumberIsNull()
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        var result = service.UpdateRecyclingFacility(
            facilityId: 1,
            name: "Green Recycling Center",
            address: "123 Main Street",
            city: "Springfield",
            state: "IL",
            zipCode: "62701",
            phoneNumber: null);

        Assert.Null(result.PhoneNumber);
    }

    [Fact]
    public void UpdateRecyclingFacility_WithFacilityIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        var exception = Record.Exception(() => service.UpdateRecyclingFacility(
            facilityId: 999,
            name: "Brand New Facility",
            address: "1 Nowhere Rd",
            city: "Capital City",
            state: "IL",
            zipCode: "62704",
            phoneNumber: null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateRecyclingFacility_DoesNotChangeTotalFixtureCount()
    {
        var fixture = CreateExistingFacilitiesFixture();
        var countBefore = fixture.Count;
        var service = CreateService(fixture);

        service.UpdateRecyclingFacility(
            facilityId: 1,
            name: "Green Recycling Center",
            address: "123 Main Street",
            city: "Springfield",
            state: "IL",
            zipCode: "62701",
            phoneNumber: "(555) 123-4567");

        Assert.Equal(countBefore, fixture.Count);
    }

    [Theory]
    [InlineData(null, "123 Main Street", "Springfield", "IL", "62701")]
    [InlineData("", "123 Main Street", "Springfield", "IL", "62701")]
    [InlineData("   ", "123 Main Street", "Springfield", "IL", "62701")]
    [InlineData("Green Recycling Center", null, "Springfield", "IL", "62701")]
    [InlineData("Green Recycling Center", "", "Springfield", "IL", "62701")]
    [InlineData("Green Recycling Center", "   ", "Springfield", "IL", "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", null, "IL", "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", "", "IL", "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", "   ", "IL", "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", "Springfield", null, "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", "Springfield", "", "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", "Springfield", "   ", "62701")]
    [InlineData("Green Recycling Center", "123 Main Street", "Springfield", "IL", null)]
    [InlineData("Green Recycling Center", "123 Main Street", "Springfield", "IL", "")]
    [InlineData("Green Recycling Center", "123 Main Street", "Springfield", "IL", "   ")]
    public void UpdateRecyclingFacility_WithMissingRequiredField_ThrowsArgumentException(
        string? name, string? address, string? city, string? state, string? zipCode)
    {
        var service = CreateService(CreateExistingFacilitiesFixture());

        Assert.ThrowsAny<ArgumentException>(() => service.UpdateRecyclingFacility(
            facilityId: 1,
            name: name!,
            address: address!,
            city: city!,
            state: state!,
            zipCode: zipCode!,
            phoneNumber: "(555) 123-4567"));
    }
}
