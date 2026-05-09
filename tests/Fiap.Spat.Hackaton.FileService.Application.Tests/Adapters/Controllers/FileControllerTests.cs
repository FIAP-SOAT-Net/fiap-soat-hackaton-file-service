using FluentAssertions;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers;
using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fiap.Spat.Hackaton.FileService.Application.Tests.Adapters.Controllers;

public class FileControllerTests
{
    [Fact]
    public async Task UploadAsync_ShouldReturnCreated_WhenHandlerSucceeds()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        uploadHandlerMock
            .Setup(x => x.Handle(It.IsAny<UploadFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok());

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);
        var request = new UploadFileRequest("f.pdf", [1], "application/pdf", 1);

        // Act
        var result = await controller.UploadAsync(request, CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task UploadAsync_ShouldReturnBadRequest_WhenHandlerFails()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        uploadHandlerMock
            .Setup(x => x.Handle(It.IsAny<UploadFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Fail("invalid"));

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.UploadAsync(new UploadFileRequest("f", [1], "text/plain", 1), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnOk_WhenHandlerSucceeds()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        getHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<GetFileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(new FileDocument { FileName = "a.pdf" }));

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.GetAsync(new GetFileByIdQuery("id"), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var valueResult = result as IValueHttpResult;
        valueResult.Should().NotBeNull();
        valueResult!.Value.Should().BeOfType<FileDocument>();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNotFound_WhenHandlerFails()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        getHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<GetFileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Fail<FileDocument>("not found"));

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.GetAsync(new GetFileByIdQuery("missing"), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnOkWithProjectedData_WhenHandlerSucceeds()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        var files = new[]
        {
            new FileDocument { FileName = "x.pdf", ContentType = "application/pdf", Size = 1, UploadedAt = DateTime.UtcNow },
            new FileDocument { FileName = "y.png", ContentType = "image/png", Size = 2, UploadedAt = DateTime.UtcNow }
        };

        listHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<GetFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok<IEnumerable<FileDocument>>(files));

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.ListAsync(new GetFilesQuery(), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var valueResult = result as IValueHttpResult;
        valueResult.Should().NotBeNull();

        var payload = valueResult!.Value as IEnumerable<object>;
        payload.Should().NotBeNull();
        payload!.Should().HaveCount(2);

        var first = payload!.First();
        first.GetType().GetProperty("FileName")!.GetValue(first).Should().Be("x.pdf");
    }

    [Fact]
    public async Task ListAsync_ShouldReturnBadRequest_WhenHandlerFails()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        listHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<GetFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Fail<IEnumerable<FileDocument>>("error"));

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.ListAsync(new GetFilesQuery(), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenHandlerSucceeds()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        deleteHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<DeleteFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok());

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.DeleteAsync(new DeleteFileCommand("id"), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenHandlerFails()
    {
        // Arrange
        var uploadHandlerMock = new Mock<IUploadFileHandler>();
        var getHandlerMock = new Mock<IGetFileHandler>();
        var listHandlerMock = new Mock<IListFilesHandler>();
        var deleteHandlerMock = new Mock<IDeleteFileHandler>();

        deleteHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<DeleteFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Fail("not found"));

        var controller = new FileController(uploadHandlerMock.Object, getHandlerMock.Object, listHandlerMock.Object, deleteHandlerMock.Object);

        // Act
        var result = await controller.DeleteAsync(new DeleteFileCommand("missing"), CancellationToken.None);

        // Assert
        var statusResult = result as IStatusCodeHttpResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
}
