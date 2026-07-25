using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-09 (GetSubmissionPeriods). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing submission periods) directly, so every model/run is exercised
// against this same fixed fixture rather than whatever arbitrary data each model would
// otherwise invent.
public class GetSubmissionPeriodsServiceTests
{
    private static List<SubmissionPeriod> CreateFixture()
    {
        return new List<SubmissionPeriod>
        {
            new SubmissionPeriod
            {
                SubmissionPeriodId = 1,
                FiscalYear = 2026,
                Period = "Q1",
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                IsCurrent = false
            },
            new SubmissionPeriod
            {
                SubmissionPeriodId = 2,
                FiscalYear = 2026,
                Period = "Q2",
                StartDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                IsCurrent = false
            },
            new SubmissionPeriod
            {
                SubmissionPeriodId = 3,
                FiscalYear = 2026,
                Period = "Q3",
                StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 7, 31, 23, 59, 59, DateTimeKind.Utc),
                IsCurrent = true
            },
            new SubmissionPeriod
            {
                SubmissionPeriodId = 4,
                FiscalYear = 2025,
                Period = "Q4",
                StartDate = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                IsCurrent = false
            }
        };
    }

    private static IGetSubmissionPeriodsService CreateService(IEnumerable<SubmissionPeriod> periods)
    {
        return new GetSubmissionPeriodsService(periods);
    }

    [Fact]
    public void GetSubmissionPeriods_WithNoFilters_ReturnsNonNullResult()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetSubmissionPeriods_WithNoFilters_ReturnsAllSeededEntries()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: null).ToList();

        Assert.Equal(fixture.Count, result.Count);
    }

    [Fact]
    public void GetSubmissionPeriods_ReturnsOnlyEntriesFromSeededData()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seededIds = fixture.Select(p => p.SubmissionPeriodId).ToHashSet();

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: null);

        foreach (SubmissionPeriod period in result)
        {
            Assert.Contains(period.SubmissionPeriodId, seededIds);
        }
    }

    [Fact]
    public void GetSubmissionPeriods_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: null);

        foreach (SubmissionPeriod period in result)
        {
            var seeded = fixture.FirstOrDefault(p => p.SubmissionPeriodId == period.SubmissionPeriodId);
            Assert.NotNull(seeded);
            Assert.Equal(seeded!.FiscalYear, period.FiscalYear);
            Assert.Equal(seeded.Period, period.Period);
            Assert.Equal(seeded.StartDate, period.StartDate);
            Assert.Equal(seeded.EndDate, period.EndDate);
            Assert.Equal(seeded.IsCurrent, period.IsCurrent);
        }
    }

    [Fact]
    public void GetSubmissionPeriods_FilterByFiscalYear_ReturnsOnlyMatchingFiscalYear()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: 2026, isCurrent: null);

        Assert.All(result, period => Assert.Equal(2026, period.FiscalYear));
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetSubmissionPeriods_FilterByFiscalYearWithNoMatches_ReturnsEmptyNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: 2099, isCurrent: null);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetSubmissionPeriods_FilterByIsCurrentTrue_ReturnsOnlyCurrentPeriod()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: true).ToList();

        Assert.All(result, period => Assert.True(period.IsCurrent));
        Assert.Single(result);
        Assert.Equal(3, result[0].SubmissionPeriodId);
    }

    [Fact]
    public void GetSubmissionPeriods_FilterByIsCurrentFalse_ReturnsOnlyNonCurrentPeriods()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: false);

        Assert.All(result, period => Assert.False(period.IsCurrent));
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetSubmissionPeriods_WithFiscalYearAndIsCurrentCombined_ReturnsMatchingPeriod()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: 2026, isCurrent: true).ToList();

        Assert.Single(result);
        Assert.Equal(3, result[0].SubmissionPeriodId);
    }

    [Fact]
    public void GetSubmissionPeriods_WithEmptyDataSet_ReturnsEmptyNotNull()
    {
        var service = CreateService(new List<SubmissionPeriod>());

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: null);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetSubmissionPeriods_ReturnsSubmissionPeriodsWithUniqueIds()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetSubmissionPeriods(fiscalYear: null, isCurrent: null).ToList();

        var ids = result.Select(p => p.SubmissionPeriodId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }
}
