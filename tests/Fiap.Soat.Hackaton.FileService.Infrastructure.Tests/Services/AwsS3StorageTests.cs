using Fiap.Soat.Hackaton.FileService.Infrastructure.Services;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Tests.Services;

public class AwsS3StorageTests
{
    [Fact(Skip = "TODO: Revisit AwsS3Storage tests when bucket-existence behavior is abstracted from EnsureBucketExistsAsync extension.")]
    public void UploadFileAsync_ShouldUseConfiguredBucketAndReturnSuccess()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "TODO: Revisit AwsS3Storage tests when bucket-existence behavior is abstracted from EnsureBucketExistsAsync extension.")]
    public void UploadFileAsync_ShouldUseDefaultBucket_WhenNotConfigured()
    {
        // Arrange

        // Act

        // Assert
    }
}
