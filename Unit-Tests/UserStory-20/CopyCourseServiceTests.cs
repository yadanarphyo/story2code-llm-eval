using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-20 (CopyCourse). Compiled directly against the generated Implementation
// project. Per Prompts/rules-file rule 3, the service's constructor takes the candidate dataset
// directly, where TDataModel is CourseCopyResult (the type the method returns).
//
// Design note: to default a new copy's title to "Copy of {original title}" and to know the
// original course exists at all, the service needs some notion of the source course being copied
// from - but rule 3 pins the constructor's item type to the RETURN type (CourseCopyResult), and
// there is no separate "Course" model referenced by this story. These tests therefore seed the
// candidate dataset with CourseCopyResult entries whose CourseId/Title double as the identity/
// title of an existing, copyable course (Status/CopiedFromCourseId/CreatedAt on those seed entries
// are otherwise irrelevant to a lookup-by-source-id). This is the most direct way the pinned
// contract can express "known existing courses," but it is a genuine spec ambiguity, not a
// certainty - unlike file-type ambiguities, however, the "Copy of {title}" default IS explicitly
// documented, so it is treated as a hard, testable requirement rather than a soft judgment call.
public class CopyCourseServiceTests
{
    private static List<CourseCopyResult> CreateExistingCoursesFixture()
    {
        return new List<CourseCopyResult>
        {
            new CourseCopyResult
            {
                CourseId = 501,
                CopiedFromCourseId = 0,
                Title = "Introduction to Data Packaging",
                Status = "Published",
                CreatedAt = new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new CourseCopyResult
            {
                CourseId = 502,
                CopiedFromCourseId = 0,
                Title = "Advanced Metadata Modeling",
                Status = "Draft",
                CreatedAt = new DateTime(2026, 7, 5, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static ICopyCourseService CreateService(IEnumerable<CourseCopyResult> existingCourses)
    {
        return new CopyCourseService(existingCourses);
    }

    [Fact]
    public void CopyCourse_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, null, null);

        Assert.NotNull(result);
    }

    [Fact]
    public void CopyCourse_ReturnsCopiedFromCourseIdMatchingSourceCourseId()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, null, null);

        Assert.Equal(501, result.CopiedFromCourseId);
    }

    [Fact]
    public void CopyCourse_AssignsNewCourseIdDifferentFromSource()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, null, null);

        Assert.NotEqual(501, result.CourseId);
    }

    [Fact]
    public void CopyCourse_WithValidInput_AssignsPositiveCourseId()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, null, null);

        Assert.True(result.CourseId > 0);
    }

    [Fact]
    public void CopyCourse_WithTitleProvided_PreservesProvidedTitle()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, "My Custom Copy Title", null);

        Assert.Equal("My Custom Copy Title", result.Title);
    }

    [Fact]
    public void CopyCourse_WithoutTitleProvided_DefaultsToCopyOfOriginalTitle()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, null, null);

        Assert.Equal("Copy of Introduction to Data Packaging", result.Title);
    }

    [Fact]
    public void CopyCourse_WithWhitespaceTitle_ReturnsNonEmptyTitle()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, "   ", null);

        Assert.False(string.IsNullOrWhiteSpace(result.Title));
    }

    [Fact]
    public void CopyCourse_SetsStatusToDraft()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(501, null, null);

        Assert.Equal("Draft", result.Status);
    }

    [Fact]
    public void CopyCourse_SetsRecentCreatedAt()
    {
        var service = CreateService(CreateExistingCoursesFixture());
        var before = DateTime.UtcNow;

        var result = service.CopyCourse(501, null, null);

        var after = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= before.AddSeconds(-1) && result.CreatedAt <= after.AddSeconds(1),
            $"Expected CreatedAt ({result.CreatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void CopyCourse_WithStartDateProvided_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() =>
            service.CopyCourse(501, null, new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc)));

        Assert.Null(exception);
    }

    [Fact]
    public void CopyCourse_WithoutStartDate_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.CopyCourse(501, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void CopyCourse_AssignsCourseIdNotCollidingWithExistingCourses()
    {
        var existingCourses = CreateExistingCoursesFixture();
        var existingIds = existingCourses.Select(c => c.CourseId).ToHashSet();
        var service = CreateService(existingCourses);

        var result = service.CopyCourse(501, null, null);

        Assert.DoesNotContain(result.CourseId, existingIds);
    }

    [Fact]
    public void CopyCourse_CalledTwiceOnSameServiceInstance_ReturnsDifferentCourseIds()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var first = service.CopyCourse(501, null, null);
        var second = service.CopyCourse(502, null, null);

        Assert.NotEqual(first.CourseId, second.CourseId);
    }

    [Fact]
    public void CopyCourse_CalledTwiceForSameSource_ReturnsDifferentCourseIds()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var first = service.CopyCourse(501, null, null);
        var second = service.CopyCourse(501, null, null);

        Assert.NotEqual(first.CourseId, second.CourseId);
    }

    [Fact]
    public void CopyCourse_DoesNotMutateExistingCoursesFixture()
    {
        var existingCourses = CreateExistingCoursesFixture();
        var countBefore = existingCourses.Count;
        var service = CreateService(existingCourses);

        service.CopyCourse(501, null, null);

        Assert.Equal(countBefore, existingCourses.Count);
    }

    [Fact]
    public void CopyCourse_WithSourceCourseIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.CopyCourse(999, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void CopyCourse_WithSourceCourseIdNotInDataset_ReturnsNonEmptyTitle()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.CopyCourse(999, null, null);

        Assert.False(string.IsNullOrWhiteSpace(result.Title));
    }

    [Fact]
    public void CopyCourse_ReturnsRecordsWithUniqueCourseIdsAcrossManyCopies()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var ids = Enumerable.Range(1, 10)
            .Select(_ => service.CopyCourse(501, null, null).CourseId)
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void CopyCourse_WithZeroSourceCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.CopyCourse(0, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void CopyCourse_WithNegativeSourceCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.CopyCourse(-1, null, null));

        Assert.Null(exception);
    }
}
