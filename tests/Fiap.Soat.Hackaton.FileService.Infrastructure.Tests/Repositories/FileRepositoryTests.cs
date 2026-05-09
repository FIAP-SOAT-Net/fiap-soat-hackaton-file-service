using Fiap.Soat.Hackaton.FileService.Infrastructure.Repositories;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Tests.Repositories;

public class FileRepositoryTests
{
    [Fact]
    public async Task InsertAsync_ShouldInsertDocument()
    {
        // Arrange
        var collectionMock = new Mock<IMongoCollection<FileDocument>>();
        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock
            .Setup(x => x.GetCollection<FileDocument>("files", It.IsAny<MongoCollectionSettings?>()))
            .Returns(collectionMock.Object);

        var repository = new FileRepository(databaseMock.Object);
        var file = new FileDocument { FileName = "a.pdf" };

        // Act
        await repository.InsertAsync(file, CancellationToken.None);

        // Assert
        collectionMock.Verify(
            x => x.InsertOneAsync(file, It.IsAny<InsertOneOptions?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenFileIdIsInvalid()
    {
        // Arrange
        var collectionMock = new Mock<IMongoCollection<FileDocument>>();
        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock
            .Setup(x => x.GetCollection<FileDocument>("files", It.IsAny<MongoCollectionSettings?>()))
            .Returns(collectionMock.Object);

        var repository = new FileRepository(databaseMock.Object);

        // Act
        var result = await repository.GetAsync("invalid-id", CancellationToken.None);

        // Assert
        result.Should().BeNull();
        collectionMock.Invocations.Should().BeEmpty();
    }

    [Fact(Skip = "TODO: Revisit after replacing MongoDB extension-method usage in FileRepository.GetAsync with an interface-mockable path.")]
    public void GetAsync_ShouldReturnDocument_WhenFileIdIsValid()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "TODO: Revisit after replacing MongoDB extension-method usage in FileRepository.GetAllAsync with an interface-mockable path.")]
    public void GetAllAsync_ShouldReturnAllDocuments()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenFileIdIsInvalid()
    {
        // Arrange
        var collectionMock = new Mock<IMongoCollection<FileDocument>>();
        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock
            .Setup(x => x.GetCollection<FileDocument>("files", It.IsAny<MongoCollectionSettings?>()))
            .Returns(collectionMock.Object);

        var repository = new FileRepository(databaseMock.Object);

        // Act
        var result = await repository.DeleteAsync("invalid", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        collectionMock.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDocumentIsDeleted()
    {
        // Arrange
        var deleteResultMock = new Mock<DeleteResult>();
        deleteResultMock.SetupGet(x => x.DeletedCount).Returns(1);

        var collectionMock = new Mock<IMongoCollection<FileDocument>>();
        collectionMock
            .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<FileDocument>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResultMock.Object);

        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock
            .Setup(x => x.GetCollection<FileDocument>("files", It.IsAny<MongoCollectionSettings?>()))
            .Returns(collectionMock.Object);

        var repository = new FileRepository(databaseMock.Object);

        // Act
        var result = await repository.DeleteAsync(ObjectId.GenerateNewId().ToString(), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNothingIsDeleted()
    {
        // Arrange
        var deleteResultMock = new Mock<DeleteResult>();
        deleteResultMock.SetupGet(x => x.DeletedCount).Returns(0);

        var collectionMock = new Mock<IMongoCollection<FileDocument>>();
        collectionMock
            .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<FileDocument>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResultMock.Object);

        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock
            .Setup(x => x.GetCollection<FileDocument>("files", It.IsAny<MongoCollectionSettings?>()))
            .Returns(collectionMock.Object);

        var repository = new FileRepository(databaseMock.Object);

        // Act
        var result = await repository.DeleteAsync(ObjectId.GenerateNewId().ToString(), CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
