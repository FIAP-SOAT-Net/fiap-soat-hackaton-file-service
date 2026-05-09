using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;

public sealed class UploadFileHandler(
    ILogger<UploadFileHandler> logger,
    IFileStorage fileStorage,
    IFileRepository fileRepository,
    IMessagePublisher messagePublisher) : IUploadFileHandler
{
    public async Task<Response> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Incoming upload Content-Type: {ContentType}", request.ContentType);
        if (request.Size == 0) return ResponseFactory.Fail("Empty file.");

        bool isPdf = string.Equals(request.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
        bool isImage = request.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

        if (!isPdf && !isImage)
        {
            return ResponseFactory.Fail("Only PDF or image files are allowed.");
        }

        var fileDocument = FileDocument.Create(
            fileName: request.Name,
            content: request.Content,
            contentType: request.ContentType,
            size: request.Size
        );
        var fileStorageResponse = await fileStorage.UploadFileAsync(fileDocument, cancellationToken);
        await fileRepository.InsertAsync(fileDocument, cancellationToken);
        var fileUploadedEvent = new
        {
            fileId = fileDocument.Id.ToString(),
            fileName = fileDocument.FileName,
            contentType = fileDocument.ContentType,
            size = fileDocument.Size,
            timestamp = DateTime.UtcNow,
            bucketName = fileStorageResponse.Data.BucketName,
            key =  fileStorageResponse.Data.Key,
        };
        string routingKey = $"file.uploaded.{fileDocument.Id}";
        await messagePublisher.PublishAsync(routingKey, message: fileUploadedEvent, cancellationToken);
        return ResponseFactory.Ok();
    }
}
