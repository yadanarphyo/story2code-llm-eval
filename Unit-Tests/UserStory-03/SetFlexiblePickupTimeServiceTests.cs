using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-03 (SetFlexiblePickupTime). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing pickup time preferences) directly, so every model/run is
// exercised against this same fixed fixture rather than whatever arbitrary data each model
// would otherwise invent.
public class SetFlexiblePickupTimeServiceTests
{
    private static List<PickupTimePreference> CreateExistingPreferencesFixture()
    {
        return new List<PickupTimePreference>
        {
            new PickupTimePreference
            {
                PreferenceId = 1,
                OrderId = 1001,
                PreferredDate = new DateOnly(2026, 7, 20),
                TimeWindowStart = new TimeOnly(9, 0),
                TimeWindowEnd = new TimeOnly(12, 0),
                IsFlexible = true,
                Notes = "Leave at front door",
                ConfirmedPickupTime = new DateTime(2026, 7, 20, 10, 0, 0, DateTimeKind.Utc),
                Status = "Confirmed",
                CreatedAt = new DateTime(2026, 7, 15, 8, 0, 0, DateTimeKind.Utc)
            },
            new PickupTimePreference
            {
                PreferenceId = 2,
                OrderId = 1002,
                PreferredDate = new DateOnly(2026, 7, 21),
                TimeWindowStart = new TimeOnly(13, 0),
                TimeWindowEnd = new TimeOnly(16, 0),
                IsFlexible = false,
                Notes = null,
                ConfirmedPickupTime = null,
                Status = "Pending",
                CreatedAt = new DateTime(2026, 7, 16, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static ISetFlexiblePickupTimeService CreateService(IEnumerable<PickupTimePreference> existingPreferences)
    {
        return new SetFlexiblePickupTimeService(existingPreferences);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithValidInput_PreservesScheduleFields()
    {
        var service = CreateService(new List<PickupTimePreference>());
        var preferredDate = new DateOnly(2026, 7, 25);
        var windowStart = new TimeOnly(9, 0);
        var windowEnd = new TimeOnly(12, 0);

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: preferredDate,
            timeWindowStart: windowStart,
            timeWindowEnd: windowEnd,
            isFlexible: true,
            notes: "Ring doorbell twice");

        Assert.Equal(2024, result.OrderId);
        Assert.Equal(preferredDate, result.PreferredDate);
        Assert.Equal(windowStart, result.TimeWindowStart);
        Assert.Equal(windowEnd, result.TimeWindowEnd);
        Assert.True(result.IsFlexible);
        Assert.Equal("Ring doorbell twice", result.Notes);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithValidInput_AssignsPositivePreferenceId()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.True(result.PreferenceId > 0);
    }

    [Fact]
    public void SetFlexiblePickupTime_AssignsPreferenceIdNotCollidingWithExistingPreferences()
    {
        var existingPreferences = CreateExistingPreferencesFixture();
        var existingIds = existingPreferences.Select(p => p.PreferenceId).ToHashSet();
        var service = CreateService(existingPreferences);

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.DoesNotContain(result.PreferenceId, existingIds);
    }

    [Fact]
    public void SetFlexiblePickupTime_CalledTwice_ReturnsDifferentPreferenceIds()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var first = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        var second = service.SetFlexiblePickupTime(
            orderId: 2025,
            preferredDate: new DateOnly(2026, 7, 26),
            timeWindowStart: new TimeOnly(13, 0),
            timeWindowEnd: new TimeOnly(17, 0),
            isFlexible: false,
            notes: null);

        Assert.NotEqual(first.PreferenceId, second.PreferenceId);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithValidInput_SetsRecentCreatedAt()
    {
        var service = CreateService(new List<PickupTimePreference>());
        var before = DateTime.UtcNow;

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        var after = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= before.AddSeconds(-1) && result.CreatedAt <= after.AddSeconds(1),
            $"Expected CreatedAt ({result.CreatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void SetFlexiblePickupTime_WithValidInput_ConfirmedPickupTimeIsNullInitially()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.Null(result.ConfirmedPickupTime);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithValidInput_StatusIsPending()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithNullNotes_DoesNotThrow()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var exception = Record.Exception(() => service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null));

        Assert.Null(exception);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithNullNotes_ResultNotesIsNull()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.Null(result.Notes);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SetFlexiblePickupTime_PreservesIsFlexibleValue(bool isFlexible)
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: isFlexible,
            notes: null);

        Assert.Equal(isFlexible, result.IsFlexible);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithEmptyExistingDataset_StillAssignsValidPreference()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var result = service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.NotNull(result);
        Assert.True(result.PreferenceId > 0);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithEqualTimeWindowStartAndEnd_DoesNotThrow()
    {
        var service = CreateService(new List<PickupTimePreference>());
        var sameTime = new TimeOnly(10, 0);

        var exception = Record.Exception(() => service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: sameTime,
            timeWindowEnd: sameTime,
            isFlexible: true,
            notes: null));

        Assert.Null(exception);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithPastPreferredDate_DoesNotThrow()
    {
        var service = CreateService(new List<PickupTimePreference>());

        var exception = Record.Exception(() => service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2020, 1, 1),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null));

        Assert.Null(exception);
    }

    [Fact]
    public void SetFlexiblePickupTime_WithSameOrderIdAsExistingPreference_DoesNotThrow()
    {
        var existingPreferences = CreateExistingPreferencesFixture();
        var service = CreateService(existingPreferences);

        var exception = Record.Exception(() => service.SetFlexiblePickupTime(
            orderId: existingPreferences[0].OrderId,
            preferredDate: new DateOnly(2026, 7, 27),
            timeWindowStart: new TimeOnly(14, 0),
            timeWindowEnd: new TimeOnly(18, 0),
            isFlexible: true,
            notes: "Updated preference for same order"));

        Assert.Null(exception);
    }

    [Fact]
    public void SetFlexiblePickupTime_DoesNotMutateExistingPreferencesFixture()
    {
        var existingPreferences = CreateExistingPreferencesFixture();
        var fixtureCountBefore = existingPreferences.Count;
        var service = CreateService(existingPreferences);

        service.SetFlexiblePickupTime(
            orderId: 2024,
            preferredDate: new DateOnly(2026, 7, 25),
            timeWindowStart: new TimeOnly(9, 0),
            timeWindowEnd: new TimeOnly(12, 0),
            isFlexible: true,
            notes: null);

        Assert.Equal(fixtureCountBefore, existingPreferences.Count);
    }
}
