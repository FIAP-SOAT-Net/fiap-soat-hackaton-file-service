using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;

public sealed class DeleteFileHandler(
    ILogger<DeleteFileHandler> logger,
    IFileRepository fileRepository,
    IMessagePublisher messagePublisher) : IDeleteFileHandler
{
    public async Task<Response> HandleAsync(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting file with ID: {FileId}", request.FileId);

        var deleted = await fileRepository.DeleteAsync(request.FileId, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("File not found or could not be deleted. Id: {FileId}", request.FileId);
            return ResponseFactory.Fail("File not found.");
        }

        var fileDeletedEvent = new
        {
            fileId = request.FileId,
            timestamp = DateTime.UtcNow
        };

        string routingKey = $"file.deleted.{request.FileId}";
        await messagePublisher.PublishAsync(routingKey, message: fileDeletedEvent, cancellationToken);

        logger.LogInformation("File deleted successfully. Id: {FileId}", request.FileId);
        return ResponseFactory.Ok();
    }
}

