using FluentAssertions;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using MongoDB.Bson;

namespace Fiap.Spat.Hackaton.FileService.Domain.Tests.Entities;

public class FileDocumentTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange

        // Act
        var document = new FileDocument();

        // Assert
        document.Id.Should().NotBe(ObjectId.Empty);
        document.FileName.Should().BeEmpty();
        document.ContentType.Should().BeEmpty();
        document.Size.Should().Be(0);
        document.Content.Should().NotBeNull().And.BeEmpty();
        document.UploadedAt.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void Create_ShouldPopulateProperties()
    {
        // Arrange
        var expectedName = "invoice.pdf";
        var expectedContentType = "application/pdf";
        var expectedContent = new byte[] { 1, 2, 3, 4 };
        const long expectedSize = 4;
        var beforeCreate = DateTime.UtcNow;

        // Act
        var document = FileDocument.Create(expectedName, expectedContent, expectedContentType, expectedSize);
        var afterCreate = DateTime.UtcNow;

        // Assert
        document.Id.Should().NotBe(ObjectId.Empty);
        document.FileName.Should().Be(expectedName);
        document.ContentType.Should().Be(expectedContentType);
        document.Content.Should().BeSameAs(expectedContent);
        document.Size.Should().Be(expectedSize);
        document.UploadedAt.Should().BeOnOrAfter(beforeCreate).And.BeOnOrBefore(afterCreate);
    }
}
