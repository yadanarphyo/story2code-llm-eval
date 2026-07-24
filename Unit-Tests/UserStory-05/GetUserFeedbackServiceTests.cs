using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-05 (GetUserFeedback). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing feedback/complaint entries) directly, so every model/run is
// exercised against this same fixed fixture rather than whatever arbitrary data each model
// would otherwise invent.
public class GetUserFeedbackServiceTests
{
    private static List<UserFeedback> CreateFixture()
    {
        return new List<UserFeedback>
        {
            new UserFeedback
            {
                FeedbackId = 1,
                UserId = 45,
                Type = "Complaint",
                Subject = "Incorrect facility hours",
                Message = "The listed hours for Green Recycling Center were wrong.",
                Status = "New",
                SubmittedDate = new DateTime(2026, 7, 1, 9, 15, 0, DateTimeKind.Utc)
            },
            new UserFeedback
            {
                FeedbackId = 2,
                UserId = 46,
                Type = "Feedback",
                Subject = "Great experience",
                Message = "The pickup process was smooth and easy.",
                Status = "Reviewed",
                SubmittedDate = new DateTime(2026, 6, 15, 14, 30, 0, DateTimeKind.Utc)
            },
            new UserFeedback
            {
                FeedbackId = 3,
                UserId = 47,
                Type = "Complaint",
                Subject = "Facility was closed early",
                Message = "Arrived before closing time but the gate was locked.",
                Status = "Resolved",
                SubmittedDate = new DateTime(2026, 7, 10, 11, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IGetUserFeedbackService CreateService(IEnumerable<UserFeedback> feedbackEntries)
    {
        return new GetUserFeedbackService(feedbackEntries);
    }

    [Fact]
    public void GetUserFeedback_WithNoFilters_ReturnsNonNullResult()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetUserFeedback(status: null, type: null, startDate: null, endDate: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetUserFeedback_WithNoFilters_ReturnsAllSeededEntries()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetUserFeedback(status: null, type: null, startDate: null, endDate: null).ToList();

        Assert.Equal(fixture.Count, result.Count);
    }

    [Fact]
    public void GetUserFeedback_ReturnsOnlyEntriesFromSeededData()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seededIds = fixture.Select(f => f.FeedbackId).ToHashSet();

        var result = service.GetUserFeedback(status: null, type: null, startDate: null, endDate: null);

        foreach (UserFeedback entry in result)
        {
            Assert.Contains(entry.FeedbackId, seededIds);
        }
    }

    [Fact]
    public void GetUserFeedback_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetUserFeedback(status: null, type: null, startDate: null, endDate: null);

        foreach (UserFeedback entry in result)
        {
            var seeded = fixture.FirstOrDefault(f => f.FeedbackId == entry.FeedbackId);
            Assert.NotNull(seeded);
            Assert.Equal(seeded!.UserId, entry.UserId);
            Assert.Equal(seeded.Type, entry.Type);
            Assert.Equal(seeded.Subject, entry.Subject);
            Assert.Equal(seeded.Message, entry.Message);
            Assert.Equal(seeded.Status, entry.Status);
            Assert.Equal(seeded.SubmittedDate, entry.SubmittedDate);
        }
    }

    [Fact]
    public void GetUserFeedback_FilterByStatus_ReturnsOnlyMatchingStatus()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetUserFeedback(status: "New", type: null, startDate: null, endDate: null);

        Assert.All(result, entry => Assert.Equal("New", entry.Status));
    }

    [Fact]
    public void GetUserFeedback_FilterByType_ReturnsOnlyMatchingType()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetUserFeedback(status: null, type: "Complaint", startDate: null, endDate: null);

        Assert.All(result, entry => Assert.Equal("Complaint", entry.Type));
    }

    [Fact]
    public void GetUserFeedback_FilterByDateRange_ReturnsOnlyEntriesWithinRange()
    {
        var service = CreateService(CreateFixture());
        var startDate = new DateOnly(2026, 7, 1);
        var endDate = new DateOnly(2026, 7, 31);

        var result = service.GetUserFeedback(status: null, type: null, startDate: startDate, endDate: endDate);

        Assert.All(result, entry =>
        {
            var submittedDateOnly = DateOnly.FromDateTime(entry.SubmittedDate);
            Assert.True(submittedDateOnly >= startDate && submittedDateOnly <= endDate);
        });
    }

    [Fact]
    public void GetUserFeedback_WithFilterNotMatchingAnything_ReturnsEmptyNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetUserFeedback(status: "NonExistentStatus", type: null, startDate: null, endDate: null);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetUserFeedback_WithEmptyDataSet_ReturnsEmptyNotNull()
    {
        var service = CreateService(new List<UserFeedback>());

        var result = service.GetUserFeedback(status: null, type: null, startDate: null, endDate: null);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetUserFeedback_ReturnsFeedbackWithUniqueIds()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetUserFeedback(status: null, type: null, startDate: null, endDate: null).ToList();

        var ids = result.Select(f => f.FeedbackId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void GetUserFeedback_WithAllFiltersCombined_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.GetUserFeedback(
            status: "New",
            type: "Complaint",
            startDate: new DateOnly(2026, 1, 1),
            endDate: new DateOnly(2026, 12, 31)));

        Assert.Null(exception);
    }
}
