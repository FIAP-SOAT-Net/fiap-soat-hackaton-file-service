using Amazon.S3;
using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.FileService.Application.Models;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;
using Microsoft.Extensions.Configuration;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Services;

public sealed class AwsS3Storage(IConfiguration configuration, IAmazonS3 amazonS3) : IFileStorage
{
    private readonly string _bucketName = configuration["S3:BucketName"] ?? "fileservice-bucket";

    public async Task<Response<FileDetails>> UploadFileAsync(FileDocument file, CancellationToken cancellationToken = default)
    {
        await amazonS3.EnsureBucketExistsAsync(_bucketName);
        string key = $"uploads/{file.Id}";
        var request = new Amazon.S3.Model.PutObjectRequest
        {
            BucketName = _bucketName, // TODO: Need to add on RabbitMQ message
            Key = key, // TODO: Need to add on RabbitMQ message
            InputStream = new MemoryStream(file.Content),
            ContentType = file.ContentType
        };
        await amazonS3.PutObjectAsync(request, cancellationToken);

        var result = new FileDetails(
            Key: key,
            BucketName: _bucketName,
            FileName: file.FileName,
            Content: file.Content,
            ContentType: file.ContentType,
            Size: file.Size,
            UploadedAt: DateTime.UtcNow
        );

        return ResponseFactory.Ok(result);
    }
}
