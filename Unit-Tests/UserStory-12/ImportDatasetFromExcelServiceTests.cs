using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-12 (ImportDatasetFromExcel). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing imported datasets) directly, so every model/run is exercised
// against this same fixed fixture rather than whatever arbitrary data each model would otherwise
// invent.
//
// Design note: unlike US-11's JSON file (naturally text), an Excel workbook (.xlsx/.xls) is a
// binary format, so the service-level parameter here is modeled as `byte[] fileContent` — the
// raw bytes of the uploaded workbook — rather than IFormFile/Stream (rule 3 forbids ASP.NET
// hosting types and async signatures) or `string` (would misrepresent binary content).
//
// Design note: these tests deliberately do NOT attempt to construct a byte-valid .xlsx workbook
// (that would require picking a specific Excel-parsing library, e.g. ClosedXML/EPPlus/NPOI/
// OpenXML, and there is no guarantee the generated implementation uses the same one). Fixture
// "file" content below is only a plausible-looking non-empty byte sequence (starting with the
// ZIP local-file-header signature `PK\x03\x04` that real .xlsx files share, since .xlsx is a ZIP
// container) used solely to exercise the required-parameter and pass-through behavior. RowCount
// is therefore only ever asserted to be non-negative when present, never to a specific expected
// count, since no shared parser is guaranteed.
//
// Design note ("does it really save?"): as with US-11, this story's API surface only exposes a
// write method with no read-back/list method, so persistence is verified indirectly: assigned
// DatasetIds must never collide with the constructor-supplied existing dataset, and must never
// collide with each other across repeated calls on the same service instance.
public class ImportDatasetFromExcelServiceTests
{
    private static List<ImportedDataset> CreateExistingDatasetsFixture()
    {
        return new List<ImportedDataset>
        {
            new ImportedDataset
            {
                DatasetId = 1,
                Name = "Regional Sales Q1",
                SourceFormat = "Excel",
                SheetName = "Sheet1",
                Status = "Completed",
                RowCount = 1500,
                CreatedAt = new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new ImportedDataset
            {
                DatasetId = 2,
                Name = "Inventory Snapshot",
                SourceFormat = "JSON",
                SheetName = null,
                Status = "Completed",
                RowCount = 640,
                CreatedAt = new DateTime(2026, 5, 12, 10, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IImportDatasetFromExcelService CreateService(IEnumerable<ImportedDataset> existingDatasets)
    {
        return new ImportDatasetFromExcelService(existingDatasets);
    }

    private static byte[] CreatePlausibleWorkbookBytes()
    {
        var header = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var padding = new byte[256];
        return header.Concat(padding).ToArray();
    }

    [Fact]
    public void ImportDatasetFromExcel_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        Assert.NotNull(result);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithValidInput_AssignsPositiveDatasetId()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        Assert.True(result.DatasetId > 0);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithValidInput_SetsSourceFormatToExcel()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        Assert.Equal("Excel", result.SourceFormat);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithValidInput_SetsNonEmptyStatus()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        Assert.False(string.IsNullOrWhiteSpace(result.Status));
        Assert.NotEqual("Failed", result.Status);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithValidInput_SetsRecentCreatedAt()
    {
        var service = CreateService(new List<ImportedDataset>());
        var before = DateTime.UtcNow;

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        var after = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= before.AddSeconds(-1) && result.CreatedAt <= after.AddSeconds(1),
            $"Expected CreatedAt ({result.CreatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void ImportDatasetFromExcel_WithSheetNameProvided_PreservesSheetName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Q2 Data", "Regional Sales Q2");

        Assert.Equal("Q2 Data", result.SheetName);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithoutSheetName_DoesNotThrow()
    {
        var service = CreateService(new List<ImportedDataset>());

        var exception = Record.Exception(() =>
            service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), null, "Regional Sales Q2"));

        Assert.Null(exception);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithDatasetNameProvided_PreservesDatasetName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        Assert.Equal("Regional Sales Q2", result.Name);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithoutDatasetName_ReturnsNonEmptyName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", null);

        Assert.False(string.IsNullOrWhiteSpace(result.Name));
    }

    [Fact]
    public void ImportDatasetFromExcel_WithWhitespaceDatasetName_ReturnsNonEmptyName()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "   ");

        Assert.False(string.IsNullOrWhiteSpace(result.Name));
    }

    [Fact]
    public void ImportDatasetFromExcel_WithValidInput_RowCountIsNotNegativeWhenPresent()
    {
        var service = CreateService(new List<ImportedDataset>());

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "Regional Sales Q2");

        Assert.True(!result.RowCount.HasValue || result.RowCount.Value >= 0);
    }

    [Fact]
    public void ImportDatasetFromExcel_WithNullFileContent_ThrowsArgumentException()
    {
        var service = CreateService(new List<ImportedDataset>());

        Assert.ThrowsAny<ArgumentException>(() => service.ImportDatasetFromExcel(null!, "Sheet1", "Some Dataset"));
    }

    [Fact]
    public void ImportDatasetFromExcel_WithEmptyFileContent_ThrowsArgumentException()
    {
        var service = CreateService(new List<ImportedDataset>());

        Assert.ThrowsAny<ArgumentException>(() => service.ImportDatasetFromExcel(Array.Empty<byte>(), "Sheet1", "Some Dataset"));
    }

    [Fact]
    public void ImportDatasetFromExcel_AssignsDatasetIdNotCollidingWithExistingDatasets()
    {
        var existingDatasets = CreateExistingDatasetsFixture();
        var existingIds = existingDatasets.Select(d => d.DatasetId).ToHashSet();
        var service = CreateService(existingDatasets);

        var result = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "New Dataset");

        Assert.DoesNotContain(result.DatasetId, existingIds);
    }

    [Fact]
    public void ImportDatasetFromExcel_CalledTwiceOnSameServiceInstance_ReturnsDifferentDatasetIds()
    {
        var service = CreateService(new List<ImportedDataset>());

        var first = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "First Dataset");
        var second = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet2", "Second Dataset");

        Assert.NotEqual(first.DatasetId, second.DatasetId);
    }

    [Fact]
    public void ImportDatasetFromExcel_CalledTwiceOnSameServiceInstance_SecondDoesNotCollideWithPreloadedDatasetIds()
    {
        var existingDatasets = CreateExistingDatasetsFixture();
        var existingIds = existingDatasets.Select(d => d.DatasetId).ToHashSet();
        var service = CreateService(existingDatasets);

        var first = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "First Dataset");
        var second = service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet2", "Second Dataset");

        Assert.DoesNotContain(first.DatasetId, existingIds);
        Assert.DoesNotContain(second.DatasetId, existingIds);
        Assert.NotEqual(first.DatasetId, second.DatasetId);
    }

    [Fact]
    public void ImportDatasetFromExcel_DoesNotMutateExistingDatasetsFixture()
    {
        var existingDatasets = CreateExistingDatasetsFixture();
        var countBefore = existingDatasets.Count;
        var service = CreateService(existingDatasets);

        service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", "New Dataset");

        Assert.Equal(countBefore, existingDatasets.Count);
    }

    [Fact]
    public void ImportDatasetFromExcel_ReturnsRecordsWithUniqueDatasetIdsAcrossManyImports()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var ids = Enumerable.Range(1, 10)
            .Select(i => service.ImportDatasetFromExcel(CreatePlausibleWorkbookBytes(), "Sheet1", $"Dataset {i}").DatasetId)
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void ImportDatasetFromExcel_WithLargeFileContent_DoesNotThrow()
    {
        var service = CreateService(new List<ImportedDataset>());
        var largeContent = new byte[] { 0x50, 0x4B, 0x03, 0x04 }.Concat(new byte[1_000_000]).ToArray();

        var exception = Record.Exception(() => service.ImportDatasetFromExcel(largeContent, "Sheet1", "Large Dataset"));

        Assert.Null(exception);
    }
}
