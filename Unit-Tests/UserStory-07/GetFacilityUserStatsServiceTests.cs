using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-07 (GetFacilityUserStats). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing facility user stats) directly, so every model/run is exercised
// against this same fixed fixture rather than whatever arbitrary data each model would otherwise
// invent.
public class GetFacilityUserStatsServiceTests
{
    private static List<FacilityUserStats> CreateFixture()
    {
        return new List<FacilityUserStats>
        {
            new FacilityUserStats
            {
                FacilityId = 1,
                TotalVisits = 342,
                PeakDayOfWeek = "Saturday",
                PeakHourRange = "10:00-12:00",
                AggregatedUserAvailability = new List<AvailabilityStat>
                {
                    new AvailabilityStat
                    {
                        DayOfWeek = "Saturday",
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(13, 0),
                        UserCount = 58
                    }
                }
            },
            new FacilityUserStats
            {
                FacilityId = 2,
                TotalVisits = 120,
                PeakDayOfWeek = "Sunday",
                PeakHourRange = "14:00-16:00",
                AggregatedUserAvailability = new List<AvailabilityStat>
                {
                    new AvailabilityStat
                    {
                        DayOfWeek = "Sunday",
                        StartTime = new TimeOnly(14, 0),
                        EndTime = new TimeOnly(16, 0),
                        UserCount = 20
                    }
                }
            }
        };
    }

    private static IGetFacilityUserStatsService CreateService(IEnumerable<FacilityUserStats> stats)
    {
        return new GetFacilityUserStatsService(stats);
    }

    [Fact]
    public void GetFacilityUserStats_WithValidFacilityId_ReturnsNonNullResult()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetFacilityUserStats_ReturnsStatsForRequestedFacilityId()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null);

        Assert.Equal(1, result.FacilityId);
    }

    [Fact]
    public void GetFacilityUserStats_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(f => f.FacilityId == 2);

        var result = service.GetFacilityUserStats(facilityId: 2, startDate: null, endDate: null);

        Assert.Equal(seeded.TotalVisits, result.TotalVisits);
        Assert.Equal(seeded.PeakDayOfWeek, result.PeakDayOfWeek);
        Assert.Equal(seeded.PeakHourRange, result.PeakHourRange);
    }

    [Fact]
    public void GetFacilityUserStats_PreservesAggregatedUserAvailabilityDetails()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(f => f.FacilityId == 1);

        var result = service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null);

        Assert.Equal(seeded.AggregatedUserAvailability.Count, result.AggregatedUserAvailability.Count);
        var seededSlot = seeded.AggregatedUserAvailability[0];
        var resultSlot = result.AggregatedUserAvailability.First(s => s.DayOfWeek == seededSlot.DayOfWeek);
        Assert.Equal(seededSlot.StartTime, resultSlot.StartTime);
        Assert.Equal(seededSlot.EndTime, resultSlot.EndTime);
        Assert.Equal(seededSlot.UserCount, resultSlot.UserCount);
    }

    [Fact]
    public void GetFacilityUserStats_AggregatedUserAvailability_IsNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null);

        Assert.NotNull(result.AggregatedUserAvailability);
    }

    [Fact]
    public void GetFacilityUserStats_WithNullDateFilters_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null));

        Assert.Null(exception);
    }

    [Fact]
    public void GetFacilityUserStats_WithDateRangeProvided_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.GetFacilityUserStats(
            facilityId: 1,
            startDate: new DateOnly(2026, 1, 1),
            endDate: new DateOnly(2026, 12, 31)));

        Assert.Null(exception);
    }

    [Fact]
    public void GetFacilityUserStats_WithFacilityIdNotMatchingAnyStats_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.GetFacilityUserStats(facilityId: 999, startDate: null, endDate: null));

        Assert.Null(exception);
    }

    [Fact]
    public void GetFacilityUserStats_WithEmptyDataSet_DoesNotThrow()
    {
        var service = CreateService(new List<FacilityUserStats>());

        var exception = Record.Exception(() => service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null));

        Assert.Null(exception);
    }

    [Fact]
    public void GetFacilityUserStats_DifferentFacilityIds_ReturnDistinctResults()
    {
        var service = CreateService(CreateFixture());

        var first = service.GetFacilityUserStats(facilityId: 1, startDate: null, endDate: null);
        var second = service.GetFacilityUserStats(facilityId: 2, startDate: null, endDate: null);

        Assert.NotEqual(first.FacilityId, second.FacilityId);
    }
}
