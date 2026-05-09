using FluentAssertions;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.Spat.Hackaton.FileService.Application.Tests.UseCases.Delete;

public class DeleteFileHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryDoesNotDelete()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DeleteFileHandler>>();
        var repositoryMock = new Mock<IFileRepository>();
        var publisherMock = new Mock<IMessagePublisher>();
        publisherMock.SetReturnsDefault(Task.CompletedTask);

        repositoryMock
            .Setup(x => x.DeleteAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteFileHandler(loggerMock.Object, repositoryMock.Object, publisherMock.Object);
        var command = new DeleteFileCommand("missing");

        // Act
        var response = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Reasons.Should().ContainSingle(x => x.Message == "File not found.");
        publisherMock.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishEventAndReturnSuccess_WhenDeleted()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DeleteFileHandler>>();
        var repositoryMock = new Mock<IFileRepository>();
        var publisherMock = new Mock<IMessagePublisher>();
        publisherMock.SetReturnsDefault(Task.CompletedTask);

        repositoryMock
            .Setup(x => x.DeleteAsync("id-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteFileHandler(loggerMock.Object, repositoryMock.Object, publisherMock.Object);
        var command = new DeleteFileCommand("id-1");

        // Act
        var response = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        publisherMock.Invocations.Should().ContainSingle();
        publisherMock.Invocations[0].Arguments[0].Should().Be("file.deleted.id-1");
    }
}
