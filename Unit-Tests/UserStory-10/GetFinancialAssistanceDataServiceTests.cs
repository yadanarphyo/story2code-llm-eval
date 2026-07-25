using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-10 (GetFinancialAssistanceData). Compiled directly against the
// generated Implementation project. Per Prompts/rules-file rule 3, the service's constructor
// takes the candidate dataset directly.
//
// Design note: the documented response is an envelope (FinancialAssistanceDataResponse)
// wrapping a filtered/paginated list of FinancialAssistanceRecord plus a LastUpdatedDate.
// Filtering and pagination only make sense over the granular records, so the candidate dataset
// here is IEnumerable<FinancialAssistanceRecord> (the type the envelope's Records list holds),
// not IEnumerable<FinancialAssistanceDataResponse>. LastUpdatedDate isn't part of the seeded
// dataset - it's implementation-computed - so these tests only assert it is set, not its value.
//
// Design note: the data model has no explicit FiscalYear field, only AwardDate, so "fiscalYear"
// filtering requires the implementation to derive it from AwardDate. All seeded dates fall in
// Jan-Sep, where both a plain calendar-year mapping and the standard US federal fiscal year
// (Oct 1 of year-1 through Sep 30 of year) agree on the same fiscal year, avoiding a brittle
// assumption about which convention the implementation picks.
public class GetFinancialAssistanceDataServiceTests
{
    private static List<FinancialAssistanceRecord> CreateFixture()
    {
        return new List<FinancialAssistanceRecord>
        {
            new FinancialAssistanceRecord
            {
                AwardId = "ASST-2026-000345",
                RecipientName = "Springfield Community Services",
                Agency = "Department of Health and Human Services",
                AwardAmount = 125000.00m,
                AwardDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new FinancialAssistanceRecord
            {
                AwardId = "ASST-2026-000346",
                RecipientName = "Lakeside Youth Center",
                Agency = "Department of Health and Human Services",
                AwardAmount = 87500.50m,
                AwardDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new FinancialAssistanceRecord
            {
                AwardId = "ASST-2026-000347",
                RecipientName = "Riverside Housing Authority",
                Agency = "Department of Housing and Urban Development",
                AwardAmount = 250000.00m,
                AwardDate = new DateTime(2026, 5, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new FinancialAssistanceRecord
            {
                AwardId = "ASST-2025-000210",
                RecipientName = "Oakwood Senior Services",
                Agency = "Department of Health and Human Services",
                AwardAmount = 64200.75m,
                AwardDate = new DateTime(2025, 2, 11, 0, 0, 0, DateTimeKind.Utc)
            },
            new FinancialAssistanceRecord
            {
                AwardId = "ASST-2026-000348",
                RecipientName = "Green Valley Food Bank",
                Agency = "Department of Agriculture",
                AwardAmount = 45000.00m,
                AwardDate = new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc)
            },
            new FinancialAssistanceRecord
            {
                AwardId = "ASST-2025-000211",
                RecipientName = "Metro Transit Access Program",
                Agency = "Department of Transportation",
                AwardAmount = 310000.00m,
                AwardDate = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IGetFinancialAssistanceDataService CreateService(IEnumerable<FinancialAssistanceRecord> records)
    {
        return new GetFinancialAssistanceDataService(records);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithNoFilters_ReturnsNonNullResponse()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithNoFilters_ReturnsNonNullRecords()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        Assert.NotNull(result.Records);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithNoPaginationParams_ReturnsAllSeededRecords()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        Assert.Equal(fixture.Count, result.Records.Count());
    }

    [Fact]
    public void GetFinancialAssistanceData_ReturnsOnlyRecordsFromSeededData()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seededIds = fixture.Select(r => r.AwardId).ToHashSet();

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        foreach (FinancialAssistanceRecord record in result.Records)
        {
            Assert.Contains(record.AwardId, seededIds);
        }
    }

    [Fact]
    public void GetFinancialAssistanceData_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(r => r.AwardId == "ASST-2026-000345");

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);
        var record = result.Records.First(r => r.AwardId == "ASST-2026-000345");

        Assert.Equal(seeded.RecipientName, record.RecipientName);
        Assert.Equal(seeded.Agency, record.Agency);
        Assert.Equal(seeded.AwardAmount, record.AwardAmount);
        Assert.Equal(seeded.AwardDate, record.AwardDate);
    }

    [Fact]
    public void GetFinancialAssistanceData_PreservesDecimalAwardAmountPrecision()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(r => r.AwardId == "ASST-2026-000346");

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);
        var record = result.Records.First(r => r.AwardId == "ASST-2026-000346");

        Assert.Equal(87500.50m, record.AwardAmount);
        Assert.Equal(seeded.AwardAmount, record.AwardAmount);
    }

    [Fact]
    public void GetFinancialAssistanceData_FilterByAgency_ReturnsOnlyMatchingAgency()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(
            agency: "Department of Health and Human Services", fiscalYear: null, page: null, pageSize: null);

        Assert.All(result.Records, record => Assert.Equal("Department of Health and Human Services", record.Agency));
        Assert.Equal(3, result.Records.Count());
    }

    [Fact]
    public void GetFinancialAssistanceData_FilterByFiscalYear_ReturnsOnlyMatchingFiscalYear()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: 2025, page: null, pageSize: null);

        Assert.All(result.Records, record => Assert.Equal(2025, record.AwardDate.Year));
        Assert.Equal(2, result.Records.Count());
    }

    [Fact]
    public void GetFinancialAssistanceData_WithPageSize_LimitsRecordCount()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: 1, pageSize: 2);

        Assert.True(result.Records.Count() <= 2);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithPageBeyondAvailableData_ReturnsEmptyRecordsNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: 999, pageSize: 2);

        Assert.NotNull(result.Records);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithFilterNotMatchingAnything_ReturnsEmptyRecordsNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(
            agency: "Nonexistent Agency", fiscalYear: null, page: null, pageSize: null);

        Assert.NotNull(result.Records);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithEmptyDataSet_ReturnsEmptyRecordsNotNull()
    {
        var service = CreateService(new List<FinancialAssistanceRecord>());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        Assert.NotNull(result);
        Assert.NotNull(result.Records);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void GetFinancialAssistanceData_ReturnsRecordsWithUniqueAwardIds()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        var ids = result.Records.Select(r => r.AwardId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void GetFinancialAssistanceData_LastUpdatedDate_IsNotDefault()
    {
        var service = CreateService(CreateFixture());

        var result = service.GetFinancialAssistanceData(agency: null, fiscalYear: null, page: null, pageSize: null);

        Assert.NotEqual(default(DateTime), result.LastUpdatedDate);
    }

    [Fact]
    public void GetFinancialAssistanceData_WithAllFiltersCombined_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.GetFinancialAssistanceData(
            agency: "Department of Health and Human Services",
            fiscalYear: 2026,
            page: 1,
            pageSize: 5));

        Assert.Null(exception);
    }
}
