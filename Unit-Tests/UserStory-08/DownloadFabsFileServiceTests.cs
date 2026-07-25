using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-08 (DownloadFabsFile). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing FABS file download descriptors, one per submission) directly,
// so every model/run is exercised against this same fixed fixture rather than whatever
// arbitrary data each model would otherwise invent.
public class DownloadFabsFileServiceTests
{
    private static List<FabsFileDownload> CreateFixture()
    {
        return new List<FabsFileDownload>
        {
            new FabsFileDownload
            {
                SubmissionId = 501,
                FileName = "fabs_submission_501.csv",
                FileSizeInBytes = 204800L,
                ContentType = "text/csv",
                DownloadUrl = "https://files.example.gov/fabs/501/fabs_submission_501.csv"
            },
            new FabsFileDownload
            {
                SubmissionId = 502,
                FileName = "fabs_submission_502.csv",
                // Deliberately larger than int.MaxValue (~2.1GB) to verify the implementation
                // truly carries this field as `long` rather than truncating/overflowing it.
                FileSizeInBytes = 5_368_709_120L,
                ContentType = "text/csv",
                DownloadUrl = "https://files.example.gov/fabs/502/fabs_submission_502.csv"
            }
        };
    }

    private static IDownloadFabsFileService CreateService(IEnumerable<FabsFileDownload> downloads)
    {
        return new DownloadFabsFileService(downloads);
    }

    [Fact]
    public void DownloadFabsFile_WithValidSubmissionId_ReturnsNonNullResult()
    {
        var service = CreateService(CreateFixture());

        var result = service.DownloadFabsFile(501);

        Assert.NotNull(result);
    }

    [Fact]
    public void DownloadFabsFile_ReturnsFileForRequestedSubmissionId()
    {
        var service = CreateService(CreateFixture());

        var result = service.DownloadFabsFile(501);

        Assert.Equal(501, result.SubmissionId);
    }

    [Fact]
    public void DownloadFabsFile_PreservesSeededFieldValues()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(f => f.SubmissionId == 501);

        var result = service.DownloadFabsFile(501);

        Assert.Equal(seeded.FileName, result.FileName);
        Assert.Equal(seeded.ContentType, result.ContentType);
        Assert.Equal(seeded.DownloadUrl, result.DownloadUrl);
    }

    [Fact]
    public void DownloadFabsFile_PreservesLargeFileSizeInBytes()
    {
        var fixture = CreateFixture();
        var service = CreateService(fixture);
        var seeded = fixture.First(f => f.SubmissionId == 502);

        var result = service.DownloadFabsFile(502);

        Assert.Equal(seeded.FileSizeInBytes, result.FileSizeInBytes);
    }

    [Fact]
    public void DownloadFabsFile_DifferentSubmissionIds_ReturnDistinctResults()
    {
        var service = CreateService(CreateFixture());

        var first = service.DownloadFabsFile(501);
        var second = service.DownloadFabsFile(502);

        Assert.NotEqual(first.SubmissionId, second.SubmissionId);
    }

    [Fact]
    public void DownloadFabsFile_WithSubmissionIdNotMatchingAnyRecord_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.DownloadFabsFile(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadFabsFile_WithEmptyDataSet_DoesNotThrow()
    {
        var service = CreateService(new List<FabsFileDownload>());

        var exception = Record.Exception(() => service.DownloadFabsFile(501));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadFabsFile_WithZeroSubmissionId_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.DownloadFabsFile(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadFabsFile_WithNegativeSubmissionId_DoesNotThrow()
    {
        var service = CreateService(CreateFixture());

        var exception = Record.Exception(() => service.DownloadFabsFile(-1));

        Assert.Null(exception);
    }
}
