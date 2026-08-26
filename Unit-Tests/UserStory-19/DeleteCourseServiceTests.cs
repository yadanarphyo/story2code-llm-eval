using System;
using System.Collections.Generic;
using Implementation.Models;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-19 (DeleteCourse). Compiled directly against the generated
// Implementation project.
//
// Design note: as with US-14 (DeleteDataset), this story's response is HTTP 204 No Content, so
// rule 3's "TDataModel is the data model class the method returns" has no return value to bind
// to. The story's "# Data Model" section pins `Course` (same shape as US-17's Course: CourseId,
// Title, Description, StartDate, EndDate, Location, Capacity, Status, CreatedAt) as the candidate
// dataset the delete operates over, so the constructor is shaped `DeleteCourseService(IEnumerable
// <Course> courses)` and the tests build fixtures against that same shape.
//
// Design note ("does it really delete?"): the method is void with no read-back method anywhere in
// this story's API surface, so there is no way to deterministically observe deletion through the
// public contract alone. These tests are limited to "does not throw" / idempotency-style checks.
public class DeleteCourseServiceTests
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
            },
            new Course
            {
                CourseId = 503,
                Title = "Data Visualization Fundamentals",
                Description = "Hands-on workshop with real datasets",
                StartDate = new DateTime(2026, 11, 3, 9, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 11, 3, 16, 0, 0, DateTimeKind.Utc),
                Location = "Virtual",
                Capacity = 50,
                Status = "Published",
                CreatedAt = new DateTime(2026, 7, 10, 9, 0, 0, DateTimeKind.Utc)
            }
        };
    }

    private static IDeleteCourseService CreateService(IEnumerable<Course> courses)
    {
        return new DeleteCourseService(courses);
    }

    [Fact]
    public void DeleteCourse_WithValidCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.DeleteCourse(501));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithUnknownCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.DeleteCourse(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_CalledTwiceForSameId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() =>
        {
            service.DeleteCourse(501);
            service.DeleteCourse(501);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithZeroCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.DeleteCourse(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithNegativeCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());

        var exception = Record.Exception(() => service.DeleteCourse(-1));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_DeletingSeveralDifferentIdsInTurn_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCoursesFixture());
        var courseIds = new[] { 501, 502, 503 };

        var exception = Record.Exception(() =>
        {
            foreach (var id in courseIds)
            {
                service.DeleteCourse(id);
            }
        });

        Assert.Null(exception);
    }
}
