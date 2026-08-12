using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-18 (UpdateCourse). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing courses, in their current state) directly.
//
// Design note: only `courseId` is Required; every Body field is optional (PUT-style partial
// update), and the documented response (CourseUpdateResult) only surfaces CourseId/Title/Status/
// UpdatedAt — description/startDate/endDate/location/capacity have no observable effect in the
// response shape, so they can only be exercised for "does not throw" coverage.
public class UpdateCourseServiceTests
{
    private static List<CourseUpdateResult> CreateExistingCoursesFixture()
    {
        return new List<CourseUpdateResult>
        {
            new CourseUpdateResult
            {
                CourseId = 501,
                Title = "Introduction to Data Packaging",
                Status = "Published",
                UpdatedAt = new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new CourseUpdateResult
            {
                CourseId = 502,
                Title = "Advanced Metadata Modeling",
                Status = "Draft",
                UpdatedAt = new DateTime(2026, 7, 5, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IUpdateCourseService CreateService(IEnumerable<CourseUpdateResult> existingCourses)
    {
        return new UpdateCourseService(existingCourses);
    }

    private static readonly DateTime ValidStartDate = new DateTime(2026, 9, 10, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void UpdateCourse_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.UpdateCourse(501, "Introduction to Data Packaging (Updated)", null, null, null, null, null);

        Assert.NotNull(result);
    }

    [Fact]
    public void UpdateCourse_WithValidInput_ReturnsMatchingCourseId()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.UpdateCourse(501, "Introduction to Data Packaging (Updated)", null, null, null, null, null);

        Assert.Equal(501, result.CourseId);
    }

    [Fact]
    public void UpdateCourse_WithTitleProvided_UpdatesTitle()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.UpdateCourse(501, "Introduction to Data Packaging (Updated)", null, null, null, null, null);

        Assert.Equal("Introduction to Data Packaging (Updated)", result.Title);
    }

    [Fact]
    public void UpdateCourse_WithTitleOmitted_PreservesExistingTitle()
    {
        var fixture = CreateExistingCoursesFixture();
        var seeded = fixture.First(c => c.CourseId == 501);
        var service = CreateService(fixture);

        var result = service.UpdateCourse(501, null, "Updated description", null, null, null, null);

        Assert.Equal(seeded.Title, result.Title);
    }

    [Fact]
    public void UpdateCourse_SetsRecentUpdatedAt()
    {
        var service = CreateService(CreateExistingCoursesFixture());
        var before = DateTime.UtcNow;

        var result = service.UpdateCourse(501, "Introduction to Data Packaging (Updated)", null, null, null, null, null);

        var after = DateTime.UtcNow;
        Assert.True(result.UpdatedAt >= before.AddSeconds(-1) && result.UpdatedAt <= after.AddSeconds(1),
            $"Expected UpdatedAt ({result.UpdatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void UpdateCourse_ReturnsNonEmptyStatus()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var result = service.UpdateCourse(501, "Introduction to Data Packaging (Updated)", null, null, null, null, null);

        Assert.False(string.IsNullOrWhiteSpace(result.Status));
    }

    [Fact]
    public void UpdateCourse_WithAllOptionalFieldsNull_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.UpdateCourse(501, null, null, null, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_WithDescriptionProvided_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() =>
            service.UpdateCourse(501, null, "New description", null, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_WithStartAndEndDateProvided_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() =>
            service.UpdateCourse(501, null, null, ValidStartDate, ValidStartDate.AddHours(3), null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_WithLocationAndCapacityProvided_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() =>
            service.UpdateCourse(501, null, null, null, null, "Room 305", 40));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_WithEndDateBeforeStartDate_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() =>
            service.UpdateCourse(501, null, null, ValidStartDate, ValidStartDate.AddDays(-1), null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_WithCourseIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.UpdateCourse(999, "Brand New Title", null, null, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_DoesNotChangeTotalFixtureCount()
    {
        var fixture = CreateExistingCoursesFixture();
        var countBefore = fixture.Count;
        var service = CreateService(fixture);

        service.UpdateCourse(501, "Introduction to Data Packaging (Updated)", null, null, null, null, null);

        Assert.Equal(countBefore, fixture.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateCourse_WithWhitespaceOrEmptyTitleProvided_DoesNotThrow(string title)
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.UpdateCourse(501, title, null, null, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void UpdateCourse_UpdatingDifferentCourses_ReturnsDistinctCourseIds()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var first = service.UpdateCourse(501, "Updated 501", null, null, null, null, null);
        var second = service.UpdateCourse(502, "Updated 502", null, null, null, null, null);

        Assert.NotEqual(first.CourseId, second.CourseId);
    }

    [Fact]
    public void UpdateCourse_WithNegativeCapacity_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.UpdateCourse(501, null, null, null, null, null, -5));

        Assert.Null(exception);
    }
}
