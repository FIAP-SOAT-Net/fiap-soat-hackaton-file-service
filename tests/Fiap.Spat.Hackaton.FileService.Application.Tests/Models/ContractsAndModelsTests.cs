using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;
using FluentAssertions;

namespace Fiap.Spat.Hackaton.FileService.Application.Tests.Models;

public class ContractsAndModelsTests
{
    [Fact]
    public void UploadFileRequest_ShouldStoreValues()
    {
        // Arrange
        var content = new byte[] { 1, 2, 3 };

        // Act
        var request = new UploadFileRequest("file.pdf", content, "application/pdf", 3);

        // Assert
        request.Name.Should().Be("file.pdf");
        request.Content.Should().BeSameAs(content);
        request.ContentType.Should().Be("application/pdf");
        request.Size.Should().Be(3);
    }

    [Fact]
    public void FileDetails_ShouldStoreValues()
    {
        // Arrange
        var content = new byte[] { 9, 8 };
        var uploadedAt = DateTime.UtcNow;

        // Act
        var details = new FileDetails("key", "bucket", "name", content, "image/png", 2, uploadedAt);

        // Assert
        details.Key.Should().Be("key");
        details.BucketName.Should().Be("bucket");
        details.FileName.Should().Be("name");
        details.Content.Should().BeSameAs(content);
        details.ContentType.Should().Be("image/png");
        details.Size.Should().Be(2);
        details.UploadedAt.Should().Be(uploadedAt);
    }

    [Fact]
    public void UploadFileCommand_ShouldStoreValues()
    {
        // Arrange
        var content = new byte[] { 4, 5 };

        // Act
        var command = new UploadFileCommand("pic.jpg", content, "image/jpeg", 2);

        // Assert
        command.Name.Should().Be("pic.jpg");
        command.Content.Should().BeSameAs(content);
        command.ContentType.Should().Be("image/jpeg");
        command.Size.Should().Be(2);
    }

    [Fact]
    public void DeleteFileCommand_ShouldStoreValues()
    {
        // Arrange

        // Act
        var command = new DeleteFileCommand("id-123");

        // Assert
        command.FileId.Should().Be("id-123");
    }

    [Fact]
    public void GetFileByIdQuery_ShouldStoreValues()
    {
        // Arrange

        // Act
        var query = new GetFileByIdQuery("abc");

        // Assert
        query.Id.Should().Be("abc");
    }

    [Fact]
    public void GetFilesQuery_ShouldBeInstantiable()
    {
        // Arrange

        // Act
        var query = new GetFilesQuery();

        // Assert
        query.Should().NotBeNull();
    }
}
