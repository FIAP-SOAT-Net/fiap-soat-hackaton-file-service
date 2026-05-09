using FluentAssertions;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Presenters;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fiap.Spat.Hackaton.FileService.Application.Tests.Adapters.Presenters;

public class ActionResultPresenterTests
{
    [Fact]
    public void ToActionResult_ShouldMapResponseStatusCode()
    {
        // Arrange
        var response = ResponseFactory.Fail("validation", HttpStatusCode.BadRequest);

        // Act
        ActionResult actionResult = ActionResultPresenter.ToActionResult(response);

        // Assert
        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        objectResult.Value.Should().Be(response);
    }

    [Fact]
    public void ToActionResult_Generic_ShouldMapResponseStatusCode()
    {
        // Arrange
        var response = ResponseFactory.Ok("payload", HttpStatusCode.Accepted);

        // Act
        ActionResult actionResult = ActionResultPresenter.ToActionResult(response);

        // Assert
        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        objectResult.Value.Should().Be(response);
    }
}
