using System;
using System.Collections.Generic;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-14 (DeleteDataset). Compiled directly against the generated
// Implementation project.
//
// Design note: the response is HTTP 204 No Content (no body), so rule 3's "TDataModel is the
// data model class the method returns" has no return value to bind to. The story's "# Data
// Model" section pins `Dataset` (DatasetId, Name, Description, Publisher, Status, CreatedAt) as
// the candidate dataset the delete operates over, so the constructor is shaped
// `DeleteDatasetService(IEnumerable<Dataset> datasets)` and the tests build fixtures against
// that same shape.
//
// Design note ("does it really delete?"): because the method is void and there is no read-back
// method anywhere in this story's API surface, there is no way to deterministically observe
// through the public contract alone whether a given ID was actually removed from a backing store
// (unlike the Import stories, deletion leaves no return value to inspect). These tests are
// therefore necessarily limited to "does not throw" / idempotency-style checks; they cannot prove
// persistence the way the write-with-response stories can.
public class DeleteDatasetServiceTests
{
    private static List<Dataset> CreateExistingDatasetsFixture()
    {
        return new List<Dataset>
        {
            new Dataset
            {
                DatasetId = 101,
                Name = "Municipal Budget 2026",
                Description = "Q1-Q2 municipal spending figures",
                Publisher = "City of Springfield",
                Status = "Published",
                CreatedAt = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new Dataset
            {
                DatasetId = 102,
                Name = "Regional Sales Q2",
                Description = null,
                Publisher = "Springfield Chamber of Commerce",
                Status = "Draft",
                CreatedAt = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc)
            },
            new Dataset
            {
                DatasetId = 103,
                Name = "Public Transit Ridership 2026",
                Description = "Monthly ridership counts by route",
                Publisher = "City of Springfield",
                Status = "Published",
                CreatedAt = new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IDeleteDatasetService CreateService(IEnumerable<Dataset> datasets)
    {
        return new DeleteDatasetService(datasets);
    }

    [Fact]
    public void DeleteDataset_WithValidDatasetId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(101));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithUnknownDatasetId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_CalledTwiceForSameId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() =>
        {
            service.DeleteDataset(101);
            service.DeleteDataset(101);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithZeroDatasetId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithNegativeDatasetId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(-1));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_DeletingSeveralDifferentIdsInTurn_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetsFixture());
        var datasetIds = new[] { 101, 102, 103 };

        var exception = Record.Exception(() =>
        {
            foreach (var id in datasetIds)
            {
                service.DeleteDataset(id);
            }
        });

        Assert.Null(exception);
    }
}
