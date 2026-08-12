using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-16 (SearchPublicDatasets). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset directly.
//
// Design note: the documented response is an envelope (DatasetSearchResponse) wrapping a
// filtered/paginated list of DatasetSearchResult plus TotalResults/Page/PageSize. Filtering and
// pagination only make sense over the granular records, so the candidate dataset here is
// IEnumerable<DatasetSearchResult> (the type the envelope's Results list holds).
public class SearchPublicDatasetsServiceTests
{
    private static List<DatasetSearchResult> CreateFixture()
    {
        return new List<DatasetSearchResult>
        {
            new DatasetSearchResult
            {
                DatasetId = 101,
                Title = "Municipal Budget 2026",
                Publisher = "City of Springfield",
                Tags = new[] { "budget", "municipal", "2026" },
                PublishedAt = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new DatasetSearchResult
            {
                DatasetId = 102,
                Title = "Regional Sales Q2",
                Publisher = "Acme Analytics",
                Tags = new[] { "sales", "regional" },
                PublishedAt = new DateTime(2026, 6, 15, 9, 0, 0, DateTimeKind.Utc)
            },
            new DatasetSearchResult
            {
                DatasetId = 103,
                Title = "Municipal Water Usage",
                Publisher = "City of Springfield",
                Tags = new[] { "water", "municipal" },
                PublishedAt = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new DatasetSearchResult
            {
                DatasetId = 104,
                Title = "National Parks Visitor Counts",
                Publisher = null,
                Tags = null,
                PublishedAt = new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static ISearchPublicDatasetsService CreateService(IEnumerable<DatasetSearchResult> datasets)
    {
        return new SearchPublicDatasetsService(datasets);
    }

    [Fact]
    public void SearchPublicDatasets_WithValidQuery_ReturnsNonNullResponse()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: null, pageSize: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void SearchPublicDatasets_WithValidQuery_ReturnsNonNullResults()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: null, pageSize: null);

        Assert.NotNull(result.Results);
    }

    [Fact]
    public void SearchPublicDatasets_FilterByTitle_ReturnsOnlyMatchingDatasets()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: null, pageSize: null);

        Assert.All(result.Results, r => Assert.Contains("Municipal", r.Title));
        Assert.Equal(2, result.Results.Count());
    }

    [Fact]
    public void SearchPublicDatasets_FilterByQueryNotMatchingAnything_ReturnsEmptyResultsNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Nonexistent Term Xyz", page: null, pageSize: null);

        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
    }

    [Fact]
    public void SearchPublicDatasets_TotalResultsReflectsFullMatchCountRegardlessOfPageSize()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: 1, pageSize: 1);

        Assert.Equal(2, result.TotalResults);
        Assert.Single(result.Results);
    }

    [Fact]
    public void SearchPublicDatasets_WithoutPagination_DefaultsPageToOne()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: null, pageSize: null);

        Assert.Equal(1, result.Page);
    }

    [Fact]
    public void SearchPublicDatasets_WithoutPagination_DefaultsPageSizeToTwenty()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: null, pageSize: null);

        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public void SearchPublicDatasets_WithPageSizeProvided_LimitsResultCount()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("a", page: 1, pageSize: 2);

        Assert.True(result.Results.Count() <= 2);
    }

    [Fact]
    public void SearchPublicDatasets_WithPageBeyondAvailableData_ReturnsEmptyResultsNotNull()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("Municipal", page: 999, pageSize: 10);

        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
    }

    [Fact]
    public void SearchPublicDatasets_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(d => d.DatasetId == 101);

        var result = service.SearchPublicDatasets("Municipal Budget 2026", page: null, pageSize: null);
        var match = result.Results.First(r => r.DatasetId == 101);

        Assert.Equal(seeded.Title, match.Title);
        Assert.Equal(seeded.Publisher, match.Publisher);
        Assert.Equal(seeded.PublishedAt, match.PublishedAt);
    }

    [Fact]
    public void SearchPublicDatasets_WithNullPublisherAndTags_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.SearchPublicDatasets("National Parks", page: null, pageSize: null));

        Assert.Null(exception);
    }

    [Fact]
    public void SearchPublicDatasets_WithEmptyDataset_ReturnsEmptyResultsNotNull()
    {
        var service = CreateService(new List<DatasetSearchResult>());

        var result = service.SearchPublicDatasets("anything", page: null, pageSize: null);

        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SearchPublicDatasets_WithMissingQuery_ThrowsArgumentException(string? query)
    {
        var service = CreateService(CreateFixture());

        Assert.ThrowsAny<ArgumentException>(() => service.SearchPublicDatasets(query!, page: null, pageSize: null));
    }

    [Fact]
    public void SearchPublicDatasets_ReturnsRecordsWithUniqueDatasetIds()
    {
        var service = CreateService(CreateFixture());

        var result = service.SearchPublicDatasets("a", page: null, pageSize: null);

        var ids = result.Results.Select(r => r.DatasetId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void SearchPublicDatasets_WithAllParametersCombined_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.SearchPublicDatasets("Municipal", page: 1, pageSize: 5));

        Assert.Null(exception);
    }
}
