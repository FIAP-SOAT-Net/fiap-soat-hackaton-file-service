using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.Spat.Hackaton.FileService.Application.Tests.UseCases.Upload;

public class UploadFileHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFileIsEmpty()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UploadFileHandler>>();
        var storageMock = new Mock<IFileStorage>();
        var repositoryMock = new Mock<IFileRepository>();
        var publisherMock = new Mock<IMessagePublisher>();

        var handler = new UploadFileHandler(loggerMock.Object, storageMock.Object, repositoryMock.Object, publisherMock.Object);
        var command = new UploadFileCommand("empty.pdf", [], "application/pdf", 0);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Reasons.Should().ContainSingle(x => x.Message == "Empty file.");
        storageMock.Invocations.Should().BeEmpty();
        repositoryMock.Invocations.Should().BeEmpty();
        publisherMock.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenContentTypeIsNotAllowed()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UploadFileHandler>>();
        var storageMock = new Mock<IFileStorage>();
        var repositoryMock = new Mock<IFileRepository>();
        var publisherMock = new Mock<IMessagePublisher>();

        var handler = new UploadFileHandler(loggerMock.Object, storageMock.Object, repositoryMock.Object, publisherMock.Object);
        var command = new UploadFileCommand("invalid.txt", [1, 2], "text/plain", 2);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Reasons.Should().ContainSingle(x => x.Message == "Only PDF or image files are allowed.");
        storageMock.Invocations.Should().BeEmpty();
        repositoryMock.Invocations.Should().BeEmpty();
        publisherMock.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldUploadPersistPublishAndReturnSuccess_WhenPdfIsValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UploadFileHandler>>();
        var storageMock = new Mock<IFileStorage>();
        var repositoryMock = new Mock<IFileRepository>();
        var publisherMock = new Mock<IMessagePublisher>();
        publisherMock.SetReturnsDefault(Task.CompletedTask);

        var storageDetails = new FileDetails("uploads/key", "bucket", "doc.pdf", [1, 2], "application/pdf", 2, DateTime.UtcNow);
        storageMock
            .Setup(x => x.UploadFileAsync(It.IsAny<Fiap.Spat.Hackaton.FileService.Domain.Entities.FileDocument>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(storageDetails));

        repositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<Fiap.Spat.Hackaton.FileService.Domain.Entities.FileDocument>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new UploadFileHandler(loggerMock.Object, storageMock.Object, repositoryMock.Object, publisherMock.Object);
        var command = new UploadFileCommand("doc.pdf", [1, 2], "application/pdf", 2);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        storageMock.Invocations.Should().ContainSingle();
        repositoryMock.Invocations.Should().ContainSingle();
        publisherMock.Invocations.Should().ContainSingle();
        publisherMock.Invocations[0].Arguments[0].ToString().Should().StartWith("file.uploaded.");
    }

    [Fact]
    public async Task Handle_ShouldAcceptImageContentType_WhenImageIsValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UploadFileHandler>>();
        var storageMock = new Mock<IFileStorage>();
        var repositoryMock = new Mock<IFileRepository>();
        var publisherMock = new Mock<IMessagePublisher>();
        publisherMock.SetReturnsDefault(Task.CompletedTask);

        var storageDetails = new FileDetails("uploads/image", "bucket", "img.png", [9], "image/png", 1, DateTime.UtcNow);
        storageMock
            .Setup(x => x.UploadFileAsync(It.IsAny<Fiap.Spat.Hackaton.FileService.Domain.Entities.FileDocument>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(storageDetails));

        repositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<Fiap.Spat.Hackaton.FileService.Domain.Entities.FileDocument>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new UploadFileHandler(loggerMock.Object, storageMock.Object, repositoryMock.Object, publisherMock.Object);
        var command = new UploadFileCommand("img.png", [9], "image/png", 1);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        storageMock.Invocations.Should().ContainSingle();
        repositoryMock.Invocations.Should().ContainSingle();
        publisherMock.Invocations.Should().ContainSingle();
    }
}
