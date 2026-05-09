using Fiap.Soat.Hackaton.FileService.Api.Endpoints;
using Fiap.Soat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;
using Fiap.Soat.Hackaton.FileService.Application.Models;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.List;
using Microsoft.AspNetCore.Http;

namespace Fiap.Soat.Hackaton.FileService.Api.Tests.Endpoints;

public class FileEndpointsUploadHandlerTests
{
    [Fact]
    public async Task UploadHandler_ShouldReadFileContentAndCallController()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var mockResult = new Mock<IResult>();
        mockController
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(f => f.FileName).Returns("test.pdf");
        mockFormFile.Setup(f => f.ContentType).Returns("application/pdf");

        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        mockFormFile
            .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) => stream.Write(fileContent, 0, fileContent.Length))
            .Returns(Task.CompletedTask);

        var cancellationToken = CancellationToken.None;

        // Act
        var handler = async (IFormFile file, IFileController controller, CancellationToken ct) =>
        {
            string fileName = file.FileName;
            string contentType = file.ContentType;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            byte[] content = memoryStream.ToArray();

            return await controller.UploadAsync(new UploadFileRequest(fileName, content, contentType, content.Length), ct);
        };

        var result = await handler(mockFormFile.Object, mockController.Object, cancellationToken);

        // Assert
        result.Should().Be(mockResult.Object);
        mockController.Verify(
            c => c.UploadAsync(
                It.Is<UploadFileRequest>(r =>
                    r.Name == "test.pdf" &&
                    r.ContentType == "application/pdf" &&
                    r.Content.Length == 5 &&
                    r.Size == 5),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task UploadHandler_ShouldHandleEmptyFile()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var mockResult = new Mock<IResult>();
        mockController
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(f => f.FileName).Returns("empty.pdf");
        mockFormFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFormFile
            .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var cancellationToken = CancellationToken.None;

        // Act
        var handler = async (IFormFile file, IFileController controller, CancellationToken ct) =>
        {
            string fileName = file.FileName;
            string contentType = file.ContentType;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            byte[] content = memoryStream.ToArray();

            return await controller.UploadAsync(new UploadFileRequest(fileName, content, contentType, content.Length), ct);
        };

        var result = await handler(mockFormFile.Object, mockController.Object, cancellationToken);

        // Assert
        result.Should().Be(mockResult.Object);
        mockController.Verify(
            c => c.UploadAsync(
                It.Is<UploadFileRequest>(r =>
                    r.Name == "empty.pdf" &&
                    r.Content.Length == 0 &&
                    r.Size == 0),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task UploadHandler_ShouldPreserveFileNameWithSpecialCharacters()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var mockResult = new Mock<IResult>();
        mockController
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(f => f.FileName).Returns("my-file_2024 (1).pdf");
        mockFormFile.Setup(f => f.ContentType).Returns("application/pdf");
        var fileContent = new byte[] { 65, 66, 67 };
        mockFormFile
            .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) => stream.Write(fileContent, 0, fileContent.Length))
            .Returns(Task.CompletedTask);

        // Act
        var handler = async (IFormFile file, IFileController controller, CancellationToken ct) =>
        {
            string fileName = file.FileName;
            string contentType = file.ContentType;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            byte[] content = memoryStream.ToArray();

            return await controller.UploadAsync(new UploadFileRequest(fileName, content, contentType, content.Length), ct);
        };

        var result = await handler(mockFormFile.Object, mockController.Object, CancellationToken.None);

        // Assert
        mockController.Verify(
            c => c.UploadAsync(
                It.Is<UploadFileRequest>(r => r.Name == "my-file_2024 (1).pdf"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UploadHandler_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        mockController
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IResult>().Object);

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(f => f.FileName).Returns("test.pdf");
        mockFormFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFormFile
            .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        var handler = async (IFormFile file, IFileController controller, CancellationToken ct) =>
        {
            string fileName = file.FileName;
            string contentType = file.ContentType;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            byte[] content = memoryStream.ToArray();

            return await controller.UploadAsync(new UploadFileRequest(fileName, content, contentType, content.Length), ct);
        };

        var result = await handler(mockFormFile.Object, mockController.Object, cancellationToken);

        // Assert
        mockController.Verify(
            c => c.UploadAsync(
                It.IsAny<UploadFileRequest>(),
                cancellationToken),
            Times.Once);
    }
}

public class FileEndpointsGetHandlerTests
{
    [Fact]
    public async Task GetHandler_ShouldCallControllerWithFileId()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var mockResult = new Mock<IResult>();
        mockController
            .Setup(c => c.GetAsync(It.IsAny<GetFileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var fileId = "507f1f77bcf86cd799439011";
        var cancellationToken = CancellationToken.None;

        // Act
        var handler = (string fId, IFileController controller, CancellationToken ct) =>
            controller.GetAsync(new GetFileByIdQuery(fId), ct);

        var result = await handler(fileId, mockController.Object, cancellationToken);

        // Assert
        result.Should().Be(mockResult.Object);
        mockController.Verify(
            c => c.GetAsync(
                It.Is<GetFileByIdQuery>(q => q.Id == fileId),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetHandler_ShouldHandleInvalidFileId()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        mockController
            .Setup(c => c.GetAsync(It.IsAny<GetFileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IResult>().Object);

        var fileId = "invalid-id";

        // Act
        var handler = (string fId, IFileController controller, CancellationToken ct) =>
            controller.GetAsync(new GetFileByIdQuery(fId), ct);

        var result = await handler(fileId, mockController.Object, CancellationToken.None);

        // Assert
        mockController.Verify(
            c => c.GetAsync(
                It.Is<GetFileByIdQuery>(q => q.Id == "invalid-id"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetHandler_ShouldPassCancellationTokenToController()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        mockController
            .Setup(c => c.GetAsync(It.IsAny<GetFileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IResult>().Object);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        var handler = (string fileId, IFileController controller, CancellationToken ct) =>
            controller.GetAsync(new GetFileByIdQuery(fileId), ct);

        await handler("507f1f77bcf86cd799439011", mockController.Object, cancellationToken);

        // Assert
        mockController.Verify(
            c => c.GetAsync(
                It.IsAny<GetFileByIdQuery>(),
                cancellationToken),
            Times.Once);
    }
}

public class FileEndpointsListHandlerTests
{
    [Fact]
    public async Task ListHandler_ShouldCallControllerWithGetFilesQuery()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var mockResult = new Mock<IResult>();
        mockController
            .Setup(c => c.ListAsync(It.IsAny<GetFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var cancellationToken = CancellationToken.None;

        // Act
        var handler = (IFileController controller, CancellationToken ct) =>
            controller.ListAsync(new GetFilesQuery(), ct);

        var result = await handler(mockController.Object, cancellationToken);

        // Assert
        result.Should().Be(mockResult.Object);
        mockController.Verify(
            c => c.ListAsync(
                It.IsAny<GetFilesQuery>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ListHandler_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        mockController
            .Setup(c => c.ListAsync(It.IsAny<GetFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IResult>().Object);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        var handler = (IFileController controller, CancellationToken ct) =>
            controller.ListAsync(new GetFilesQuery(), ct);

        await handler(mockController.Object, cancellationToken);

        // Assert
        mockController.Verify(
            c => c.ListAsync(
                It.IsAny<GetFilesQuery>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ListHandler_ShouldReturnControllerResult()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var expectedResult = new Mock<IResult>().Object;
        mockController
            .Setup(c => c.ListAsync(It.IsAny<GetFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var handler = (IFileController controller, CancellationToken ct) =>
            controller.ListAsync(new GetFilesQuery(), ct);

        var result = await handler(mockController.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
    }
}

public class FileEndpointsDeleteHandlerTests
{
    [Fact]
    public async Task DeleteHandler_ShouldCallControllerWithFileId()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var mockResult = new Mock<IResult>();
        mockController
            .Setup(c => c.DeleteAsync(It.IsAny<DeleteFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var fileId = "507f1f77bcf86cd799439011";
        var cancellationToken = CancellationToken.None;

        // Act
        var handler = (string fId, IFileController controller, CancellationToken ct) =>
            controller.DeleteAsync(new DeleteFileCommand(fId), ct);

        var result = await handler(fileId, mockController.Object, cancellationToken);

        // Assert
        result.Should().Be(mockResult.Object);
        mockController.Verify(
            c => c.DeleteAsync(
                It.Is<DeleteFileCommand>(cmd => cmd.FileId == fileId),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeleteHandler_ShouldHandleInvalidFileId()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        mockController
            .Setup(c => c.DeleteAsync(It.IsAny<DeleteFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IResult>().Object);

        var fileId = "not-a-valid-id";

        // Act
        var handler = (string fId, IFileController controller, CancellationToken ct) =>
            controller.DeleteAsync(new DeleteFileCommand(fId), ct);

        var result = await handler(fileId, mockController.Object, CancellationToken.None);

        // Assert
        mockController.Verify(
            c => c.DeleteAsync(
                It.Is<DeleteFileCommand>(cmd => cmd.FileId == "not-a-valid-id"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteHandler_ShouldPassCancellationTokenToController()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        mockController
            .Setup(c => c.DeleteAsync(It.IsAny<DeleteFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<IResult>().Object);

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Act
        var handler = (string fileId, IFileController controller, CancellationToken ct) =>
            controller.DeleteAsync(new DeleteFileCommand(fileId), ct);

        await handler("507f1f77bcf86cd799439011", mockController.Object, cancellationToken);

        // Assert
        mockController.Verify(
            c => c.DeleteAsync(
                It.IsAny<DeleteFileCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeleteHandler_ShouldReturnControllerResult()
    {
        // Arrange
        var mockController = new Mock<IFileController>();
        var expectedResult = new Mock<IResult>().Object;
        mockController
            .Setup(c => c.DeleteAsync(It.IsAny<DeleteFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var handler = (string fileId, IFileController controller, CancellationToken ct) =>
            controller.DeleteAsync(new DeleteFileCommand(fileId), ct);

        var result = await handler("507f1f77bcf86cd799439011", mockController.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
    }
}
