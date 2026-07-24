using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-06 (UploadUserSchedule). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing uploaded schedules) directly, so every model/run is exercised
// against this same fixed fixture rather than whatever arbitrary data each model would otherwise
// invent.
public class UploadUserScheduleServiceTests
{
    private static List<TimeSlot> CreateAvailabilityFixture()
    {
        return new List<TimeSlot>
        {
            new TimeSlot { DayOfWeek = "Monday", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(12, 0) },
            new TimeSlot { DayOfWeek = "Wednesday", StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(18, 0) }
        };
    }

    private static List<UserSchedule> CreateExistingSchedulesFixture()
    {
        return new List<UserSchedule>
        {
            new UserSchedule
            {
                ScheduleId = 1,
                UserId = 45,
                Availability = new List<TimeSlot>
                {
                    new TimeSlot { DayOfWeek = "Friday", StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) }
                }
            },
            new UserSchedule
            {
                ScheduleId = 2,
                UserId = 46,
                Availability = new List<TimeSlot>
                {
                    new TimeSlot { DayOfWeek = "Tuesday", StartTime = new TimeOnly(16, 0), EndTime = new TimeOnly(19, 0) }
                }
            }
        };
    }

    private static IUploadUserScheduleService CreateService(IEnumerable<UserSchedule> existingSchedules)
    {
        return new UploadUserScheduleService(existingSchedules);
    }

    [Fact]
    public void UploadUserSchedule_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(new List<UserSchedule>());

        var result = service.UploadUserSchedule(45, CreateAvailabilityFixture());

        Assert.NotNull(result);
    }

    [Fact]
    public void UploadUserSchedule_WithValidInput_PreservesUserId()
    {
        var service = CreateService(new List<UserSchedule>());

        var result = service.UploadUserSchedule(45, CreateAvailabilityFixture());

        Assert.Equal(45, result.UserId);
    }

    [Fact]
    public void UploadUserSchedule_WithValidInput_PreservesAvailabilityCount()
    {
        var availability = CreateAvailabilityFixture();
        var service = CreateService(new List<UserSchedule>());

        var result = service.UploadUserSchedule(45, availability);

        Assert.Equal(availability.Count, result.Availability.Count);
    }

    [Fact]
    public void UploadUserSchedule_WithValidInput_PreservesTimeSlotDetails()
    {
        var availability = CreateAvailabilityFixture();
        var service = CreateService(new List<UserSchedule>());

        var result = service.UploadUserSchedule(45, availability);

        foreach (var slot in availability)
        {
            var stored = result.Availability.FirstOrDefault(s => s.DayOfWeek == slot.DayOfWeek);
            Assert.NotNull(stored);
            Assert.Equal(slot.StartTime, stored!.StartTime);
            Assert.Equal(slot.EndTime, stored.EndTime);
        }
    }

    [Fact]
    public void UploadUserSchedule_WithValidInput_AssignsPositiveScheduleId()
    {
        var service = CreateService(new List<UserSchedule>());

        var result = service.UploadUserSchedule(45, CreateAvailabilityFixture());

        Assert.True(result.ScheduleId > 0);
    }

    [Fact]
    public void UploadUserSchedule_AssignsScheduleIdNotCollidingWithExistingSchedules()
    {
        var existingSchedules = CreateExistingSchedulesFixture();
        var existingIds = existingSchedules.Select(s => s.ScheduleId).ToHashSet();
        var service = CreateService(existingSchedules);

        var result = service.UploadUserSchedule(47, CreateAvailabilityFixture());

        Assert.DoesNotContain(result.ScheduleId, existingIds);
    }

    [Fact]
    public void UploadUserSchedule_CalledTwice_ReturnsDifferentScheduleIds()
    {
        var service = CreateService(new List<UserSchedule>());

        var first = service.UploadUserSchedule(45, CreateAvailabilityFixture());
        var second = service.UploadUserSchedule(46, CreateAvailabilityFixture());

        Assert.NotEqual(first.ScheduleId, second.ScheduleId);
    }

    [Fact]
    public void UploadUserSchedule_WithNullAvailability_ThrowsArgumentException()
    {
        var service = CreateService(new List<UserSchedule>());

        Assert.ThrowsAny<ArgumentException>(() => service.UploadUserSchedule(45, null!));
    }

    [Fact]
    public void UploadUserSchedule_WithEmptyAvailabilityList_DoesNotThrow()
    {
        var service = CreateService(new List<UserSchedule>());

        var exception = Record.Exception(() => service.UploadUserSchedule(45, new List<TimeSlot>()));

        Assert.Null(exception);
    }

    [Fact]
    public void UploadUserSchedule_WithSingleTimeSlot_PreservesSlot()
    {
        var service = CreateService(new List<UserSchedule>());
        var availability = new List<TimeSlot>
        {
            new TimeSlot { DayOfWeek = "Sunday", StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0) }
        };

        var result = service.UploadUserSchedule(45, availability);

        var slot = Assert.Single(result.Availability);
        Assert.Equal("Sunday", slot.DayOfWeek);
        Assert.Equal(new TimeOnly(10, 0), slot.StartTime);
        Assert.Equal(new TimeOnly(11, 0), slot.EndTime);
    }

    [Fact]
    public void UploadUserSchedule_WithSameUserIdUploadedTwice_DoesNotThrow()
    {
        var existingSchedules = CreateExistingSchedulesFixture();
        var service = CreateService(existingSchedules);

        var exception = Record.Exception(() => service.UploadUserSchedule(
            existingSchedules[0].UserId,
            CreateAvailabilityFixture()));

        Assert.Null(exception);
    }

    [Fact]
    public void UploadUserSchedule_DoesNotMutateExistingSchedulesFixture()
    {
        var existingSchedules = CreateExistingSchedulesFixture();
        var countBefore = existingSchedules.Count;
        var service = CreateService(existingSchedules);

        service.UploadUserSchedule(45, CreateAvailabilityFixture());

        Assert.Equal(countBefore, existingSchedules.Count);
    }
}
