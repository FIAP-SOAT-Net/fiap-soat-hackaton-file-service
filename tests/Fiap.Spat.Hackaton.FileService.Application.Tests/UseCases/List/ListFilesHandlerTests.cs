using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Fiap.Spat.Hackaton.FileService.Application.Tests.UseCases.List;

public class ListFilesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRepositoryFiles()
    {
        // Arrange
        var repositoryMock = new Mock<IFileRepository>();
        var files = new List<FileDocument>
        {
            new() { FileName = "a.pdf" },
            new() { FileName = "b.png" }
        };

        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(files);

        var handler = new ListFilesHandler(repositoryMock.Object);

        // Act
        var response = await handler.HandleAsync(new GetFilesQuery(), CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(files);
    }
}
