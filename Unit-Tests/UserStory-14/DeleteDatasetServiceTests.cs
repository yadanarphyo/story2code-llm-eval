using System;
using System.Collections.Generic;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-14 (DeleteDataset). Compiled directly against the generated
// Implementation project.
//
// Design note: this story has no "# Data Model" section at all (the response is HTTP 204 No
// Content, no body), so rule 3's "constructor takes IEnumerable<TDataModel>, where TDataModel is
// the data model class the method returns" has no literal data model to bind to since the method
// returns void. The most minimal, unambiguous stand-in a model could reasonably invent given only
// a bare `datasetId` in the story is a plain existing-ID collection, so the constructor here is
// modeled as `IEnumerable<int>` (existing dataset IDs) rather than any invented named class.
//
// Design note ("does it really delete?"): because the method is void and there is no read-back
// method anywhere in this story's API surface, there is no way to deterministically observe
// through the public contract alone whether a given ID was actually removed from a backing store
// (unlike the Import stories, deletion leaves no return value to inspect). These tests are
// therefore necessarily limited to "does not throw" / idempotency-style checks; they cannot prove
// persistence the way the write-with-response stories can.
public class DeleteDatasetServiceTests
{
    private static List<int> CreateExistingDatasetIdsFixture()
    {
        return new List<int> { 101, 102, 103 };
    }

    private static IDeleteDatasetService CreateService(IEnumerable<int> existingDatasetIds)
    {
        return new DeleteDatasetService(existingDatasetIds);
    }

    [Fact]
    public void DeleteDataset_WithExistingDatasetId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetIdsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(101));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithDatasetIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetIdsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithEmptyDataset_DoesNotThrow()
    {
        var service = CreateService(new List<int>());

        var exception = Record.Exception(() => service.DeleteDataset(101));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_CalledTwiceForSameId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetIdsFixture());

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
        var service = CreateService(CreateExistingDatasetIdsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithNegativeDatasetId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingDatasetIdsFixture());

        var exception = Record.Exception(() => service.DeleteDataset(-1));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_DeletingEachExistingIdInTurn_DoesNotThrow()
    {
        var fixture = CreateExistingDatasetIdsFixture();
        var service = CreateService(fixture);

        var exception = Record.Exception(() =>
        {
            foreach (var id in fixture)
            {
                service.DeleteDataset(id);
            }
        });

        Assert.Null(exception);
    }
}
