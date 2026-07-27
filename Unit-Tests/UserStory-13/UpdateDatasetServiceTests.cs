using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-13 (UpdateDataset). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing published datasets, in their current state) directly, so every
// model/run is exercised against this same fixed fixture rather than whatever arbitrary data each
// model would otherwise invent.
//
// Design note: `metadata` ("object") and `rows` ("array") are documented only generically, with
// no referenced Data Model schema for their contents (unlike, say, US-06's `availability: array`
// which is pinned to `List<TimeSlot>`). These are modeled here as `Dictionary<string, object>?`
// and `List<Dictionary<string, object>>?` respectively — the most common idiomatic .NET mapping
// for an arbitrary JSON object / array of row-shaped objects — but this is a genuine ambiguity in
// the story spec itself, not a technical constraint tests can resolve. Since DatasetUpdateResult
// (the documented response shape) does not surface metadata or rows at all, these tests can only
// verify they don't break the call, not their downstream effect.
//
// Design note: only `datasetId` is Required; every Body field is optional (PUT-style partial
// update). Tests therefore verify that omitted fields don't clobber previously-set values on the
// matching seeded record, consistent with partial-update semantics.
public class UpdateDatasetServiceTests
{
    private static List<DatasetUpdateResult> CreateExistingDatasetsFixture()
    {
        return new List<DatasetUpdateResult>
        {
            new DatasetUpdateResult
            {
                DatasetId = 101,
                Name = "Municipal Budget 2026",
                Description = "Initial submission of Q1-Q2 spending",
                Status = "Published",
                UpdatedAt = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new DatasetUpdateResult
            {
                DatasetId = 102,
                Name = "Regional Sales Q2",
                Description = null,
                Status = "Draft",
                UpdatedAt = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IUpdateDatasetService CreateService(IEnumerable<DatasetUpdateResult> existingDatasets)
    {
        return new UpdateDatasetService(existingDatasets);
    }

    [Fact]
    public void UpdateDataset_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var result = service.UpdateDataset(101, "Municipal Budget 2026 (Revised)", "Corrected figures", null, null);

        Assert.NotNull(result);
    }

    [Fact]
    public void UpdateDataset_WithValidInput_ReturnsMatchingDatasetId()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var result = service.UpdateDataset(101, "Municipal Budget 2026 (Revised)", "Corrected figures", null, null);

        Assert.Equal(101, result.DatasetId);
    }

    [Fact]
    public void UpdateDataset_WithNameProvided_UpdatesName()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var result = service.UpdateDataset(101, "Municipal Budget 2026 (Revised)", null, null, null);

        Assert.Equal("Municipal Budget 2026 (Revised)", result.Name);
    }

    [Fact]
    public void UpdateDataset_WithDescriptionProvided_UpdatesDescription()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var result = service.UpdateDataset(101, null, "Corrected figures for Q2 spending", null, null);

        Assert.Equal("Corrected figures for Q2 spending", result.Description);
    }

    [Fact]
    public void UpdateDataset_WithNameOmitted_PreservesExistingName()
    {
        var fixture = CreateExistingDatasetsFixture();
        var seeded = fixture.First(d => d.DatasetId == 101);
        var service = CreateService(fixture);

        var result = service.UpdateDataset(101, null, "Updated description only", null, null);

        Assert.Equal(seeded.Name, result.Name);
    }

    [Fact]
    public void UpdateDataset_WithDescriptionOmitted_PreservesExistingDescription()
    {
        var fixture = CreateExistingDatasetsFixture();
        var seeded = fixture.First(d => d.DatasetId == 101);
        var service = CreateService(fixture);

        var result = service.UpdateDataset(101, "Updated name only", null, null, null);

        Assert.Equal(seeded.Description, result.Description);
    }

    [Fact]
    public void UpdateDataset_WithAllOptionalFieldsNull_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.UpdateDataset(101, null, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDataset_WithMetadataProvided_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());
        var metadata = new Dictionary<string, object> { ["region"] = "Midwest", ["fiscalYear"] = 2026 };

        var exception = Record.Exception(() => service.UpdateDataset(101, null, null, metadata, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDataset_WithRowsProvided_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());
        var rows = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { ["department"] = "Finance", ["amount"] = 1000 },
            new Dictionary<string, object> { ["department"] = "Health", ["amount"] = 2000 }
        };

        var exception = Record.Exception(() => service.UpdateDataset(101, null, null, null, rows));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDataset_WithEmptyMetadataAndRows_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.UpdateDataset(
            101, null, null, new Dictionary<string, object>(), new List<Dictionary<string, object>>()));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDataset_SetsRecentUpdatedAt()
    {
        var service = CreateService(CreateExistingDatasetsFixture());
        var before = DateTime.UtcNow;

        var result = service.UpdateDataset(101, "Municipal Budget 2026 (Revised)", null, null, null);

        var after = DateTime.UtcNow;
        Assert.True(result.UpdatedAt >= before.AddSeconds(-1) && result.UpdatedAt <= after.AddSeconds(1),
            $"Expected UpdatedAt ({result.UpdatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void UpdateDataset_ReturnsNonEmptyStatus()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var result = service.UpdateDataset(101, "Municipal Budget 2026 (Revised)", null, null, null);

        Assert.False(string.IsNullOrWhiteSpace(result.Status));
    }

    [Fact]
    public void UpdateDataset_WithDatasetIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.UpdateDataset(999, "Brand New Name", null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDataset_DoesNotChangeTotalFixtureCount()
    {
        var fixture = CreateExistingDatasetsFixture();
        var countBefore = fixture.Count;
        var service = CreateService(fixture);

        service.UpdateDataset(101, "Municipal Budget 2026 (Revised)", null, null, null);

        Assert.Equal(countBefore, fixture.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDataset_WithWhitespaceOrEmptyNameProvided_DoesNotThrow(string name)
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.UpdateDataset(101, name, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDataset_UpdatingDifferentDatasets_ReturnsDistinctDatasetIds()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var first = service.UpdateDataset(101, "Updated 101", null, null, null);
        var second = service.UpdateDataset(102, "Updated 102", null, null, null);

        Assert.NotEqual(first.DatasetId, second.DatasetId);
    }
}
