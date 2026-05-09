using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using FluentAssertions;
using FluentResults;
using Moq;
using System.Net;

namespace Fiap.Spat.Hackaton.FileService.Domain.Tests.Shared;

public class ResponseOfTTests
{
    [Fact]
    public void Ok_ShouldCreateSuccessfulResponse()
    {
        // Arrange
        const string expectedValue = "ok";

        // Act
        var response = Response<string>.Ok(expectedValue);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(expectedValue);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Reasons.Should().BeEmpty();
    }

    [Fact]
    public void Ok_ShouldUseProvidedStatusCode()
    {
        // Arrange
        const string expectedValue = "created";

        // Act
        var response = Response<string>.Ok(expectedValue, HttpStatusCode.Created);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public void Fail_WithMessage_ShouldCreateFailedResponse()
    {
        // Arrange
        const string expectedMessage = "validation failed";

        // Act
        var response = Response<string>.Fail(expectedMessage);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Reasons.Should().ContainSingle(x => x.Message == expectedMessage);
    }

    [Fact]
    public void Fail_WithError_ShouldCreateFailedResponse()
    {
        // Arrange
        var error = new Error("critical error");

        // Act
        var response = Response<string>.Fail(error, HttpStatusCode.InternalServerError);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Reasons.Should().ContainSingle(x => x.Message == "critical error");
    }

    [Fact]
    public void Fail_WithErrorList_ShouldCreateFailedResponse()
    {
        // Arrange
        var mockedError1 = new Mock<IError>();
        mockedError1.SetupGet(x => x.Message).Returns("error one");

        var mockedError2 = new Mock<IError>();
        mockedError2.SetupGet(x => x.Message).Returns("error two");

        var errors = new List<IError> { mockedError1.Object, mockedError2.Object };

        // Act
        var response = Response<string>.Fail(errors, HttpStatusCode.UnprocessableEntity);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        response.Reasons.Should().HaveCount(2);
        response.Reasons.Select(x => x.Message).Should().Contain(["error one", "error two"]);
    }

    [Fact]
    public void ImplicitResultOperator_ShouldReturnInnerResult()
    {
        // Arrange
        var response = Response<string>.Ok("payload");

        // Act
        Result<string> result = response;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("payload");
    }

    [Fact]
    public void ImplicitResponseOperator_ShouldConvertSuccessfulResponse()
    {
        // Arrange
        var response = Response<string>.Ok("payload", HttpStatusCode.Accepted);

        // Act
        Response converted = response;

        // Assert
        converted.IsSuccess.Should().BeTrue();
        converted.StatusCode.Should().Be(HttpStatusCode.Accepted);
        converted.Reasons.Should().BeEmpty();
    }

    [Fact]
    public void ImplicitResponseOperator_ShouldConvertFailedResponseWithMessages()
    {
        // Arrange
        var mockedReason = new Mock<IReason>();
        mockedReason.SetupGet(x => x.Message).Returns("mapped reason");

        var result = Result.Fail<string>(mockedReason.Object.Message);
        var response = new Response<string>(result, HttpStatusCode.Conflict);

        // Act
        Response converted = response;

        // Assert
        converted.IsSuccess.Should().BeFalse();
        converted.StatusCode.Should().Be(HttpStatusCode.Conflict);
        converted.Reasons.Should().ContainSingle(x => x.Message == "mapped reason");
    }
}
