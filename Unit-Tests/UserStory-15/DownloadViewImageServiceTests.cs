using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-15 (DownloadViewImage). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset directly.
//
// Design note: unlike lookup-style GET stories (e.g. US-08 DownloadFabsFile), the response here
// (ViewImageExport) directly echoes the caller-supplied format/width/height and adds generated
// metadata (FileSizeBytes, GeneratedAt) — i.e. this is a render/generate operation keyed by an
// existing viewId, not a fixed-record lookup. The constructor-supplied candidate dataset is
// modeled as prior ViewImageExport renders (consistent with rule 3), but these tests do not rely
// on it for output correctness beyond construction, since nothing in the story documents using
// prior renders to influence a new one.
public class DownloadViewImageServiceTests
{
    private static List<ViewImageExport> CreateExistingExportsFixture()
    {
        return new List<ViewImageExport>
        {
            new ViewImageExport
            {
                ViewId = 10,
                Format = "png",
                Width = 800,
                Height = 600,
                FileSizeBytes = 204800L,
                GeneratedAt = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IDownloadViewImageService CreateService(IEnumerable<ViewImageExport> existingExports)
    {
        return new DownloadViewImageService(existingExports);
    }

    [Fact]
    public void DownloadViewImage_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, "png", 800, 600);

        Assert.NotNull(result);
    }

    [Fact]
    public void DownloadViewImage_ReturnsMatchingViewId()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, "png", 800, 600);

        Assert.Equal(42, result.ViewId);
    }

    [Fact]
    public void DownloadViewImage_WithFormatProvided_PreservesFormat()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, "jpeg", null, null);

        Assert.Equal("jpeg", result.Format);
    }

    [Fact]
    public void DownloadViewImage_WithoutFormat_DefaultsToPng()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, null, null, null);

        Assert.Equal("png", result.Format);
    }

    [Theory]
    [InlineData("png")]
    [InlineData("jpeg")]
    [InlineData("svg")]
    public void DownloadViewImage_WithDocumentedFormat_ReturnsRequestedFormat(string format)
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, format, null, null);

        Assert.Equal(format, result.Format);
    }

    [Fact]
    public void DownloadViewImage_WithWidthAndHeightProvided_PreservesWidthAndHeight()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, "png", 1024, 768);

        Assert.Equal(1024, result.Width);
        Assert.Equal(768, result.Height);
    }

    [Fact]
    public void DownloadViewImage_WithoutWidthAndHeight_ReturnsNullWidthAndHeight()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, "png", null, null);

        Assert.Null(result.Width);
        Assert.Null(result.Height);
    }

    [Fact]
    public void DownloadViewImage_SetsPositiveFileSizeBytes()
    {
        var service = CreateService(new List<ViewImageExport>());

        var result = service.DownloadViewImage(42, "png", 800, 600);

        Assert.True(result.FileSizeBytes > 0);
    }

    [Fact]
    public void DownloadViewImage_SetsRecentGeneratedAt()
    {
        var service = CreateService(new List<ViewImageExport>());
        var before = DateTime.UtcNow;

        var result = service.DownloadViewImage(42, "png", 800, 600);

        var after = DateTime.UtcNow;
        Assert.True(result.GeneratedAt >= before.AddSeconds(-1) && result.GeneratedAt <= after.AddSeconds(1),
            $"Expected GeneratedAt ({result.GeneratedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void DownloadViewImage_WithZeroViewId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingExportsFixture());

        var exception = Record.Exception(() => service.DownloadViewImage(0, "png", null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadViewImage_WithNegativeViewId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingExportsFixture());

        var exception = Record.Exception(() => service.DownloadViewImage(-1, "png", null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadViewImage_WithViewIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingExportsFixture());

        var exception = Record.Exception(() => service.DownloadViewImage(999, "png", null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadViewImage_WithWhitespaceFormat_DoesNotThrow()
    {
        var service = CreateService(new List<ViewImageExport>());

        var exception = Record.Exception(() => service.DownloadViewImage(42, "   ", null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadViewImage_WithNegativeWidthOrHeight_DoesNotThrow()
    {
        var service = CreateService(new List<ViewImageExport>());

        var exception = Record.Exception(() => service.DownloadViewImage(42, "png", -100, -50));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadViewImage_WithEmptyDataset_DoesNotThrow()
    {
        var service = CreateService(new List<ViewImageExport>());

        var exception = Record.Exception(() => service.DownloadViewImage(42, "png", 800, 600));

        Assert.Null(exception);
    }

    [Fact]
    public void DownloadViewImage_DifferentViewIds_ReturnDistinctViewIdsInResult()
    {
        var service = CreateService(new List<ViewImageExport>());

        var first = service.DownloadViewImage(1, "png", null, null);
        var second = service.DownloadViewImage(2, "png", null, null);

        Assert.NotEqual(first.ViewId, second.ViewId);
    }
}
