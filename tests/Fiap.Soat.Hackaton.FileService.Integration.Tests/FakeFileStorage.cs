using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Soat.Hackaton.FileService.Application.Models;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Integration.Tests;

public sealed class FakeFileStorage : IFileStorage
{
    public int UploadCalls { get; private set; }
    public FileDocument? LastUploadedFile { get; private set; }

    public Task<Response<FileDetails>> UploadFileAsync(FileDocument file, CancellationToken cancellationToken = default)
    {
        UploadCalls++;
        LastUploadedFile = file;

        var fileDetails = new FileDetails(
            Key: $"files/{file.Id}",
            BucketName: "fileservice-tests-bucket",
            FileName: file.FileName,
            Content: file.Content,
            ContentType: file.ContentType,
            Size: file.Size,
            UploadedAt: file.UploadedAt);

        return Task.FromResult(ResponseFactory.Ok(fileDetails));
    }
}
