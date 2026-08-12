using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-11 (ImportDatasetFromJson). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing imported datasets) directly, so every model/run is exercised
// against this same fixed fixture rather than whatever arbitrary data each model would otherwise
// invent.
//
// Design note: the user story's "file" parameter is documented as "file (application/json)" at
// the HTTP/Form Data level, but rule 3's service contract must be a plain synchronous method the
// tests call directly (no IFormFile/ASP.NET Core hosting types, no Task/Async). Per rule 5, the
// controller is responsible for reading the uploaded file and handing its content down to the
// service, so the service-level parameter here is modeled as the raw JSON text of that file:
// `string fileContent`. This is the most direct, dependency-free way to feed known JSON payloads
// into the service from a unit test.
//
// Design note: the "Response"/"Data Model" sections disagree on RowCount's nullability — the
// example response shows a populated rowCount while Status is still "Processing", but the Data
// Model table marks RowCount nullable "until processing completes". Rather than assume either
// policy, these tests treat RowCount as optionally eager: if it is non-null, it must match the
// actual number of elements in the imported JSON array; if it is null, no assertion is made about
// when it becomes available.
//
// Design note ("does it really save?"): this story's API surface only exposes a write method
// (ImportDatasetFromJson) with no corresponding read-back/list method, so there is no direct way
// to query the service's backing store after the fact. Persistence into that in-memory store is
// therefore verified indirectly but concretely: assigned DatasetIds must never collide with the
// constructor-supplied existing dataset, and must never collide with each other across repeated
// calls on the same service instance. Both are only possible if the service actually accumulates
// state internally (a real save) rather than fabricating a response with no backing store.
public class ImportDatasetFromJsonServiceTests
{
    private static List<ImportedDataset> CreateExistingDatasetsFixture()
    {
        return new List<ImportedDataset>
        {
            new ImportedDataset
            {
                DatasetId = 1,
                Name = "Municipal Budget 2025",
                SourceFormat = "JSON",
                Status = "Completed",
                RowCount = 3200,
                CreatedAt = new DateTime(2026, 1, 10, 9, 0, 0, DateTimeKind.Utc)
            },
            new ImportedDataset
            {
                DatasetId = 2,
                Name = "Water Usage Report",
                SourceFormat = "CSV",
                Status = "Completed",
                RowCount = 980,
                CreatedAt = new DateTime(2026, 2, 5, 14, 30, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IImportDatasetFromJsonService CreateService(IEnumerable<ImportedDataset> existingDatasets)
    {
        return new ImportDatasetFromJsonService(existingDatasets);
    }

    private static string CreateJsonArrayContent(int rowCount)
    {
        var rows = Enumerable.Range(1, rowCount)
            .Select(i => $"{{\"id\":{i},\"value\":\"Row {i}\"}}");
        return "[" + string.Join(",", rows) + "]";
    }

    [Fact]
    public void ImportDatasetFromJson_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(5), "Municipal Budget 2026");

        Assert.NotNull(result);
    }

    [Fact]
    public void ImportDatasetFromJson_WithValidInput_AssignsPositiveDatasetId()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(5), "Municipal Budget 2026");

        Assert.True(result.DatasetId > 0);
    }

    [Fact]
    public void ImportDatasetFromJson_WithValidInput_SetsSourceFormatToJson()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(5), "Municipal Budget 2026");

        Assert.Equal("JSON", result.SourceFormat);
    }

    [Fact]
    public void ImportDatasetFromJson_WithValidInput_SetsNonEmptyStatus()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(5), "Municipal Budget 2026");

        Assert.False(string.IsNullOrWhiteSpace(result.Status));
        Assert.NotEqual("Failed", result.Status);
    }

    [Fact]
    public void ImportDatasetFromJson_WithValidInput_SetsRecentCreatedAt()
    {
        var service = CreateService(new List<ImportedDataset>());
        var before = DateTime.UtcNow;

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(5), "Municipal Budget 2026");

        var after = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= before.AddSeconds(-1) && result.CreatedAt <= after.AddSeconds(1),
            $"Expected CreatedAt ({result.CreatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void ImportDatasetFromJson_WithDatasetNameProvided_PreservesDatasetName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(3), "Municipal Budget 2026");

        Assert.Equal("Municipal Budget 2026", result.Name);
    }

    [Fact]
    public void ImportDatasetFromJson_WithoutDatasetName_ReturnsNonEmptyName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(3), null);

        Assert.False(string.IsNullOrWhiteSpace(result.Name));
    }

    [Fact]
    public void ImportDatasetFromJson_WithWhitespaceDatasetName_DoesNotThrow()
    {
        var service = CreateService(new List<ImportedDataset>());

        var exception = Record.Exception(() => service.ImportDatasetFromJson(CreateJsonArrayContent(3), "   "));

        Assert.Null(exception);
    }

    [Fact]
    public void ImportDatasetFromJson_WithWhitespaceDatasetName_ReturnsNonEmptyName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(3), "   ");

        Assert.False(string.IsNullOrWhiteSpace(result.Name));
    }

    [Fact]
    public void ImportDatasetFromJson_WithJsonArrayOfKnownSize_RowCountMatchesArrayLengthWhenPresent()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(17), "Sample Dataset");

        if (result.RowCount.HasValue)
        {
            Assert.Equal(17, result.RowCount.Value);
        }
    }

    [Fact]
    public void ImportDatasetFromJson_WithEmptyJsonArray_DoesNotThrow()
    {
        var service = CreateService(new List<ImportedDataset>());

        var exception = Record.Exception(() => service.ImportDatasetFromJson("[]", "Empty Dataset"));

        Assert.Null(exception);
    }

    [Fact]
    public void ImportDatasetFromJson_WithEmptyJsonArray_RowCountIsNotNegative()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromJson("[]", "Empty Dataset");

        Assert.True(!result.RowCount.HasValue || result.RowCount.Value >= 0);
    }

    [Fact]
    public void ImportDatasetFromJson_WithLargeJsonArray_DoesNotThrow()
    {
        var service = CreateService(new List<ImportedDataset>());

        var exception = Record.Exception(() => service.ImportDatasetFromJson(CreateJsonArrayContent(5000), "Large Dataset"));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ImportDatasetFromJson_WithMissingFileContent_ThrowsArgumentException(string? fileContent)
    {
        var service = CreateService(new List<ImportedDataset>());

        Assert.ThrowsAny<ArgumentException>(() => service.ImportDatasetFromJson(fileContent!, "Some Dataset"));
    }

    [Fact]
    public void ImportDatasetFromJson_WithMalformedJsonContent_ThrowsArgumentException()
    {
        var service = CreateService(new List<ImportedDataset>());

        Assert.ThrowsAny<ArgumentException>(() => service.ImportDatasetFromJson("{ this is not valid json ]", "Bad Dataset"));
    }

    [Fact]
    public void ImportDatasetFromJson_AssignsDatasetIdNotCollidingWithExistingDatasets()
    {
        var existingDatasets = CreateExistingDatasetsFixture();
        var existingIds = existingDatasets.Select(d => d.DatasetId).ToHashSet();
        var service = CreateService(existingDatasets);

        var result = service.ImportDatasetFromJson(CreateJsonArrayContent(4), "New Dataset");

        Assert.DoesNotContain(result.DatasetId, existingIds);
    }

    [Fact]
    public void ImportDatasetFromJson_CalledTwiceOnSameServiceInstance_ReturnsDifferentDatasetIds()
    {
        var service = CreateService(new List<ImportedDataset>());

        var first = service.ImportDatasetFromJson(CreateJsonArrayContent(4), "First Dataset");
        var second = service.ImportDatasetFromJson(CreateJsonArrayContent(6), "Second Dataset");

        Assert.NotEqual(first.DatasetId, second.DatasetId);
    }

    [Fact]
    public void ImportDatasetFromJson_CalledTwiceOnSameServiceInstance_SecondDoesNotCollideWithPreloadedDatasetIds()
    {
        var existingDatasets = CreateExistingDatasetsFixture();
        var existingIds = existingDatasets.Select(d => d.DatasetId).ToHashSet();
        var service = CreateService(existingDatasets);

        var first = service.ImportDatasetFromJson(CreateJsonArrayContent(4), "First Dataset");
        var second = service.ImportDatasetFromJson(CreateJsonArrayContent(6), "Second Dataset");

        Assert.DoesNotContain(first.DatasetId, existingIds);
        Assert.DoesNotContain(second.DatasetId, existingIds);
        Assert.NotEqual(first.DatasetId, second.DatasetId);
    }

    [Fact]
    public void ImportDatasetFromJson_DoesNotMutateExistingDatasetsFixture()
    {
        var existingDatasets = CreateExistingDatasetsFixture();
        var countBefore = existingDatasets.Count;
        var service = CreateService(existingDatasets);

        service.ImportDatasetFromJson(CreateJsonArrayContent(4), "New Dataset");

        Assert.Equal(countBefore, existingDatasets.Count);
    }

    [Fact]
    public void ImportDatasetFromJson_ReturnsRecordsWithUniqueDatasetIdsAcrossManyImports()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var ids = Enumerable.Range(1, 10)
            .Select(i => service.ImportDatasetFromJson(CreateJsonArrayContent(i), $"Dataset {i}").DatasetId)
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }
}
