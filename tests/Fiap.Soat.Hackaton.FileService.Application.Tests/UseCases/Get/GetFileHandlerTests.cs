using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Fiap.Soat.Hackaton.FileService.Application.Tests.UseCases.Get;

public class GetFileHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenFileExists()
    {
        // Arrange
        var repositoryMock = new Mock<IFileRepository>();
        var expectedFile = new FileDocument { FileName = "file.pdf" };
        repositoryMock
            .Setup(x => x.GetAsync("id-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFile);

        var handler = new GetFileHandler(repositoryMock.Object);
        var query = new GetFileByIdQuery("id-1");

        // Act
        var response = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeSameAs(expectedFile);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenFileDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IFileRepository>();
        repositoryMock
            .Setup(x => x.GetAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((FileDocument?) null);

        var handler = new GetFileHandler(repositoryMock.Object);
        var query = new GetFileByIdQuery("missing");

        // Act
        var response = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Reasons.Should().ContainSingle(x => x.Message == "File not found.");
    }
}
