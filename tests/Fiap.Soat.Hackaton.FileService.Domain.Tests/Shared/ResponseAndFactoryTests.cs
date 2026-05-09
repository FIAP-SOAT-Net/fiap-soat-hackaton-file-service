using Fiap.Soat.Hackaton.FileService.Domain.Shared;
using FluentAssertions;
using FluentResults;
using Moq;
using System.Net;

namespace Fiap.Soat.Hackaton.FileService.Domain.Tests.Shared;

public class ResponseAndFactoryTests
{
    [Fact]
    public void ResponseOk_WithValue_ShouldCreateSuccessfulResponseObject()
    {
        // Arrange
        var value = new { Name = "file" };

        // Act
        var response = Response.Ok(value, HttpStatusCode.Created);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Data.Should().Be(value);
        response.Reasons.Should().BeEmpty();
    }

    [Fact]
    public void ResponseOk_WithoutValue_ShouldCreateSuccessfulResponse()
    {
        // Arrange

        // Act
        var response = Response.Ok(HttpStatusCode.Accepted);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Reasons.Should().BeEmpty();
    }

    [Fact]
    public void ResponseFail_WithMessage_ShouldCreateFailedResponse()
    {
        // Arrange
        const string expectedMessage = "invalid state";

        // Act
        var response = Response.Fail(expectedMessage, HttpStatusCode.BadRequest);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Reasons.Should().ContainSingle(x => x.Message == expectedMessage);
    }

    [Fact]
    public void ResponseFail_WithError_ShouldCreateFailedResponse()
    {
        // Arrange
        var error = new Error("unexpected");

        // Act
        var response = Response.Fail(error, HttpStatusCode.InternalServerError);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Reasons.Should().ContainSingle(x => x.Message == "unexpected");
    }

    [Fact]
    public void ResponseFail_WithErrorList_ShouldCreateFailedResponse()
    {
        // Arrange
        var mockedError = new Mock<IError>();
        mockedError.SetupGet(x => x.Message).Returns("first");
        var errors = new List<IError> { mockedError.Object };

        // Act
        var response = Response.Fail(errors, HttpStatusCode.UnprocessableEntity);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        response.Reasons.Should().ContainSingle(x => x.Message == "first");
    }

    [Fact]
    public void ResponseImplicitResultOperator_ShouldReturnInnerResult()
    {
        // Arrange
        var response = Response.Ok(HttpStatusCode.OK);

        // Act
        Result result = response;

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ResponseFactoryOk_WithoutValue_ShouldCreateSuccessfulResponse()
    {
        // Arrange

        // Act
        var response = ResponseFactory.Ok(HttpStatusCode.Created);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public void ResponseFactoryOk_WithValue_ShouldCreateSuccessfulResponse()
    {
        // Arrange
        const int value = 10;

        // Act
        var response = ResponseFactory.Ok(value, HttpStatusCode.Accepted);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(value);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void ResponseFactoryFail_GenericWithMessage_ShouldCreateFailedResponse()
    {
        // Arrange
        const string expectedMessage = "bad request";

        // Act
        var response = ResponseFactory.Fail<int>(expectedMessage, HttpStatusCode.BadRequest);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Reasons.Should().ContainSingle(x => x.Message == expectedMessage);
    }

    [Fact]
    public void ResponseFactoryFail_GenericWithError_ShouldCreateFailedResponse()
    {
        // Arrange
        var error = new Error("db error");

        // Act
        var response = ResponseFactory.Fail<int>(error, HttpStatusCode.InternalServerError);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Reasons.Should().ContainSingle(x => x.Message == "db error");
    }

    [Fact]
    public void ResponseFactoryFail_NonGenericWithError_ShouldCreateFailedResponse()
    {
        // Arrange
        var error = new Error("non generic error");

        // Act
        var response = ResponseFactory.Fail(error, HttpStatusCode.NotFound);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Reasons.Should().ContainSingle(x => x.Message == "non generic error");
    }

    [Fact]
    public void ResponseFactoryFail_NonGenericWithString_ShouldCreateFailedResponse()
    {
        // Arrange
        const string expectedMessage = "single message";

        // Act
        var response = ResponseFactory.Fail(expectedMessage, HttpStatusCode.Conflict);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.Reasons.Should().ContainSingle(x => x.Message == expectedMessage);
    }

    [Fact]
    public void ResponseFactoryFail_NonGenericWithStringArray_ShouldCreateFailedResponse()
    {
        // Arrange
        var messages = new[] { "first message", "second message" };

        // Act
        var response = ResponseFactory.Fail(messages, HttpStatusCode.UnprocessableEntity);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        response.Reasons.Should().HaveCount(2);
        response.Reasons.Select(x => x.Message).Should().Contain(messages);
    }

    [Fact]
    public void ResponseFactoryFail_GenericWithErrorList_ShouldCreateFailedResponse()
    {
        // Arrange
        var mockedError1 = new Mock<IError>();
        mockedError1.SetupGet(x => x.Message).Returns("e1");
        var mockedError2 = new Mock<IError>();
        mockedError2.SetupGet(x => x.Message).Returns("e2");
        var errors = new List<IError> { mockedError1.Object, mockedError2.Object };

        // Act
        var response = ResponseFactory.Fail<string>(errors, HttpStatusCode.InternalServerError);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Reasons.Should().HaveCount(2);
        response.Reasons.Select(x => x.Message).Should().Contain(["e1", "e2"]);
    }
}
