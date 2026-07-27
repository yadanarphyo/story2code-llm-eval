using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-17 (CreateCourse). Compiled directly against the generated
// Implementation project. Per Prompts/rules-file rule 3, the service's constructor takes the
// candidate dataset (pre-existing courses) directly, so every model/run is exercised against this
// same fixed fixture rather than whatever arbitrary data each model would otherwise invent.
//
// Design note ("does it really save?"): as with the account/schedule/import stories, persistence
// is verified indirectly via cross-call statefulness: assigned CourseIds must never collide with
// the constructor-supplied existing courses, and must never collide with each other across
// repeated calls on the same service instance.
public class CreateCourseServiceTests
{
    private static List<Course> CreateExistingCoursesFixture()
    {
        return new List<Course>
        {
            new Course
            {
                CourseId = 501,
                Title = "Introduction to Data Packaging",
                Description = "Beginner-friendly overview",
                StartDate = new DateTime(2026, 9, 10, 9, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc),
                Location = "Room 204",
                Capacity = 30,
                Status = "Published",
                CreatedAt = new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new Course
            {
                CourseId = 502,
                Title = "Advanced Metadata Modeling",
                Description = null,
                StartDate = new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc),
                EndDate = null,
                Location = null,
                Capacity = null,
                Status = "Draft",
                CreatedAt = new DateTime(2026, 7, 5, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static ICreateCourseService CreateService(IEnumerable<Course> existingCourses)
    {
        return new CreateCourseService(existingCourses);
    }

    private static readonly DateTime ValidStartDate = new DateTime(2026, 9, 10, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void CreateCourse_WithValidInput_ReturnsNonNullResult()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", "A course about testing", ValidStartDate, null, null, null);

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateCourse_WithValidInput_PreservesTitle()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", "A course about testing", ValidStartDate, null, null, null);

        Assert.Equal("Intro to Testing", result.Title);
    }

    [Fact]
    public void CreateCourse_WithValidInput_PreservesStartDate()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", "A course about testing", ValidStartDate, null, null, null);

        Assert.Equal(ValidStartDate, result.StartDate);
    }

    [Fact]
    public void CreateCourse_WithValidInput_AssignsPositiveCourseId()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", "A course about testing", ValidStartDate, null, null, null);

        Assert.True(result.CourseId > 0);
    }

    [Fact]
    public void CreateCourse_WithValidInput_SetsRecentCreatedAt()
    {
        var service = CreateService(new List<Course>());
        var before = DateTime.UtcNow;

        var result = service.CreateCourse("Intro to Testing", "A course about testing", ValidStartDate, null, null, null);

        var after = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= before.AddSeconds(-1) && result.CreatedAt <= after.AddSeconds(1),
            $"Expected CreatedAt ({result.CreatedAt:o}) to fall between {before:o} and {after:o}");
    }

    [Fact]
    public void CreateCourse_WithValidInput_SetsNonEmptyStatus()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", "A course about testing", ValidStartDate, null, null, null);

        Assert.False(string.IsNullOrWhiteSpace(result.Status));
    }

    [Fact]
    public void CreateCourse_WithEndDateProvided_PreservesEndDate()
    {
        var service = CreateService(new List<Course>());
        var endDate = ValidStartDate.AddHours(3);

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, endDate, null, null);

        Assert.Equal(endDate, result.EndDate);
    }

    [Fact]
    public void CreateCourse_WithoutEndDate_ReturnsNullEndDate()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, null);

        Assert.Null(result.EndDate);
    }

    [Fact]
    public void CreateCourse_WithLocationProvided_PreservesLocation()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, null, "Room 101", null);

        Assert.Equal("Room 101", result.Location);
    }

    [Fact]
    public void CreateCourse_WithoutLocation_ReturnsNullLocation()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, null);

        Assert.Null(result.Location);
    }

    [Fact]
    public void CreateCourse_WithCapacityProvided_PreservesCapacity()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, 25);

        Assert.Equal(25, result.Capacity);
    }

    [Fact]
    public void CreateCourse_WithoutCapacity_ReturnsNullCapacity()
    {
        var service = CreateService(new List<Course>());

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, null);

        Assert.Null(result.Capacity);
    }

    [Fact]
    public void CreateCourse_WithNullDescription_DoesNotThrow()
    {
        var service = CreateService(new List<Course>());

        var exception = Record.Exception(() => service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void CreateCourse_AssignsCourseIdNotCollidingWithExistingCourses()
    {
        var existingCourses = CreateExistingCoursesFixture();
        var existingIds = existingCourses.Select(c => c.CourseId).ToHashSet();
        var service = CreateService(existingCourses);

        var result = service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, null);

        Assert.DoesNotContain(result.CourseId, existingIds);
    }

    [Fact]
    public void CreateCourse_CalledTwice_ReturnsDifferentCourseIds()
    {
        var service = CreateService(new List<Course>());

        var first = service.CreateCourse("First Course", null, ValidStartDate, null, null, null);
        var second = service.CreateCourse("Second Course", null, ValidStartDate.AddDays(1), null, null, null);

        Assert.NotEqual(first.CourseId, second.CourseId);
    }

    [Fact]
    public void CreateCourse_CalledTwiceOnSameServiceInstance_SecondDoesNotCollideWithPreloadedCourseIds()
    {
        var existingCourses = CreateExistingCoursesFixture();
        var existingIds = existingCourses.Select(c => c.CourseId).ToHashSet();
        var service = CreateService(existingCourses);

        var first = service.CreateCourse("First Course", null, ValidStartDate, null, null, null);
        var second = service.CreateCourse("Second Course", null, ValidStartDate.AddDays(1), null, null, null);

        Assert.DoesNotContain(first.CourseId, existingIds);
        Assert.DoesNotContain(second.CourseId, existingIds);
        Assert.NotEqual(first.CourseId, second.CourseId);
    }

    [Fact]
    public void CreateCourse_DoesNotMutateExistingCoursesFixture()
    {
        var existingCourses = CreateExistingCoursesFixture();
        var countBefore = existingCourses.Count;
        var service = CreateService(existingCourses);

        service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, null);

        Assert.Equal(countBefore, existingCourses.Count);
    }

    [Fact]
    public void CreateCourse_ReturnsRecordsWithUniqueCourseIdsAcrossManyCreations()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var ids = Enumerable.Range(1, 10)
            .Select(i => service.CreateCourse($"Course {i}", null, ValidStartDate.AddDays(i), null, null, null).CourseId)
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateCourse_WithMissingTitle_ThrowsArgumentException(string? title)
    {
        var service = CreateService(new List<Course>());

        Assert.ThrowsAny<ArgumentException>(() => service.CreateCourse(title!, null, ValidStartDate, null, null, null));
    }

    [Fact]
    public void CreateCourse_WithEndDateBeforeStartDate_DoesNotThrow()
    {
        var service = CreateService(new List<Course>());

        var exception = Record.Exception(() =>
            service.CreateCourse("Intro to Testing", null, ValidStartDate, ValidStartDate.AddDays(-1), null, null));

        Assert.Null(exception);
    }

    [Fact]
    public void CreateCourse_WithZeroCapacity_DoesNotThrow()
    {
        var service = CreateService(new List<Course>());

        var exception = Record.Exception(() => service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, 0));

        Assert.Null(exception);
    }

    [Fact]
    public void CreateCourse_WithNegativeCapacity_DoesNotThrow()
    {
        var service = CreateService(new List<Course>());

        var exception = Record.Exception(() => service.CreateCourse("Intro to Testing", null, ValidStartDate, null, null, -5));

        Assert.Null(exception);
    }
}
