using Fiap.Soat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;
using Fiap.Soat.Hackaton.FileService.Application.Models;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.List;

namespace Fiap.Soat.Hackaton.FileService.Api.Endpoints;

public static class FileEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/files/upload", async (IFormFile file, IFileController controller, CancellationToken cancellationToken) =>
            {
                string fileName = file.FileName;
                string contentType = file.ContentType;
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                byte[] content = memoryStream.ToArray();

                return await controller.UploadAsync(new UploadFileRequest(fileName, content, contentType, content.Length), cancellationToken);
            })
            .DisableAntiforgery()
            .WithName("UploadFile")
            .WithDescription("Upload a PDF or image file");

        app.MapGet("/files/{fileId}", (string fileId, IFileController controller, CancellationToken cancellationToken) => controller.GetAsync(new GetFileByIdQuery(fileId), cancellationToken))
            .WithName("GetFile")
            .WithDescription("Download a file by ID");

        app.MapGet("/files", (IFileController controller, CancellationToken cancellationToken) => controller.ListAsync(new GetFilesQuery(), cancellationToken))
            .WithName("ListFiles")
            .WithDescription("List all uploaded files");

        app.MapDelete("/files/{fileId}", (string fileId, IFileController controller, CancellationToken cancellationToken) => controller.DeleteAsync(new DeleteFileCommand(fileId), cancellationToken))
            .WithName("DeleteFile")
            .WithDescription("Delete a file by ID");
    }
}
