using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-14 (DeleteDataset). Compiled directly against the generated
// Implementation project.
//
// Design note: this story has no "# Data Model" section at all (the response is HTTP 204 No
// Content, no body), so rule 3's "constructor takes IEnumerable<TDataModel>" has no literal data
// model to bind to. Per Prompts/rules-file rule 3, a parameterless constructor is acceptable when
// the story has no Data Model section, so the service is constructed parameterless here, following
// the story exactly as written rather than inventing an unstated dataset parameter/type.
//
// Design note ("does it really delete?"): because the method is void and there is no read-back
// method anywhere in this story's API surface, there is no way to deterministically observe
// through the public contract alone whether a given ID was actually removed from a backing store
// (unlike the Import stories, deletion leaves no return value to inspect). These tests are
// therefore necessarily limited to "does not throw" / idempotency-style checks; they cannot prove
// persistence the way the write-with-response stories can.
public class DeleteDatasetServiceTests
{
    private static IDeleteDatasetService CreateService()
    {
        return new DeleteDatasetService();
    }

    [Fact]
    public void DeleteDataset_WithValidDatasetId_DoesNotThrow()
    {
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteDataset(101));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithUnknownDatasetId_DoesNotThrow()
    {
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteDataset(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_CalledTwiceForSameId_DoesNotThrow()
    {
        var service = CreateService();

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
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteDataset(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_WithNegativeDatasetId_DoesNotThrow()
    {
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteDataset(-1));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteDataset_DeletingSeveralDifferentIdsInTurn_DoesNotThrow()
    {
        var service = CreateService();
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
