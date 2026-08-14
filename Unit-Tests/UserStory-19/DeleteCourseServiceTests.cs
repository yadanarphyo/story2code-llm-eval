using Implementation.Services;
using Xunit;

namespace Implementation.Tests;

// White-box tests for US-19 (DeleteCourse). Compiled directly against the generated
// Implementation project.
//
// Design note: as with US-14 (DeleteDataset), this story has no "# Data Model" section (the
// response is HTTP 204 No Content), so there is no named class for rule 3's constructor to bind
// to. Per Prompts/rules-file rule 3, a parameterless constructor is acceptable when the story has
// no Data Model section, so the service is constructed parameterless here, following the story
// exactly as written rather than inventing an unstated dataset parameter/type.
//
// Design note ("does it really delete?"): the method is void with no read-back method anywhere in
// this story's API surface, so there is no way to deterministically observe deletion through the
// public contract alone. These tests are limited to "does not throw" / idempotency-style checks.
public class DeleteCourseServiceTests
{
    private static IDeleteCourseService CreateService()
    {
        return new DeleteCourseService();
    }

    [Fact]
    public void DeleteCourse_WithValidCourseId_DoesNotThrow()
    {
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteCourse(501));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithUnknownCourseId_DoesNotThrow()
    {
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteCourse(999));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_CalledTwiceForSameId_DoesNotThrow()
    {
        var service = CreateService();

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
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteCourse(0));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_WithNegativeCourseId_DoesNotThrow()
    {
        var service = CreateService();

        var exception = Record.Exception(() => service.DeleteCourse(-1));

        Assert.Null(exception);
    }

    [Fact]
    public void DeleteCourse_DeletingSeveralDifferentIdsInTurn_DoesNotThrow()
    {
        var service = CreateService();
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
