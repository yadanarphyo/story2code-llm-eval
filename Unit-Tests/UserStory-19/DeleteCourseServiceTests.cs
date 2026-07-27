using System;
using System.Collections.Generic;
using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-19 (DeleteCourse). Compiled directly against the generated
// Implementation project.
//
// Design note: as with US-14 (DeleteDataset), this story has no "# Data Model" section (the
// response is HTTP 204 No Content), so there is no named class for rule 3's constructor to bind
// to. The constructor here is modeled as the minimal unambiguous stand-in: `IEnumerable<int>`
// (existing course IDs).
//
// Design note ("does it really delete?"): the method is void with no read-back method anywhere in
// this story's API surface, so there is no way to deterministically observe deletion through the
// public contract alone. These tests are limited to "does not throw" / idempotency-style checks.
public class DeleteCourseServiceTests
{
    private static List<int> CreateExistingCourseIdsFixture()
    {
        return new List<int> { 501, 502, 503 };
    }

    private static IDeleteCourseService CreateService(IEnumerable<int> existingCourseIds)
    {
        return new DeleteCourseService(existingCourseIds);
    }

    [Fact]
    public void DeleteCourse_WithExistingCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCourseIdsFixture());

        var exception = Record.Exception(() => service.DeleteCourse(501));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithCourseIdNotInDataset_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCourseIdsFixture());

        var exception = Record.Exception(() => service.DeleteCourse(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithEmptyDataset_DoesNotThrow()
    {
        var service = CreateService(new List<int>());

        var exception = Record.Exception(() => service.DeleteCourse(501));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_CalledTwiceForSameId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCourseIdsFixture());

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
        var service = CreateService(CreateExistingCourseIdsFixture());

        var exception = Record.Exception(() => service.DeleteCourse(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithNegativeCourseId_DoesNotThrow()
    {
        var service = CreateService(CreateExistingCourseIdsFixture());

        var exception = Record.Exception(() => service.DeleteCourse(-1));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_DeletingEachExistingIdInTurn_DoesNotThrow()
    {
        var fixture = CreateExistingCourseIdsFixture();
        var service = CreateService(fixture);

        var exception = Record.Exception(() =>
        {
            foreach (var id in fixture)
            {
                service.DeleteCourse(id);
            }
        });

        Assert.Null(exception);
    }
}
