using Fiap.Soat.Hackaton.FileService.Api.Services;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.Models;

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

        app.MapGet("/files/{fileId}", async (string fileId, IFileService fileService, CancellationToken cancellationToken) =>
            {
                var file = await fileService.GetFileAsync(fileId, cancellationToken);
                return file is null ? Results.NotFound() : Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("GetFile")
            .WithDescription("Download a file by ID");

        app.MapGet("/files", async (IFileService fileService, CancellationToken cancellationToken) =>
            {
                var files = await fileService.GetAllFilesAsync(cancellationToken);

                return Results.Ok(files.Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.Size,
                    f.UploadedAt
                }));
            })
            .WithName("ListFiles")
            .WithDescription("List all uploaded files");

        app.MapDelete("/files/{fileId}", async (string fileId, IFileService fileService, CancellationToken cancellationToken) =>
            {
                bool deleted = await fileService.DeleteFileAsync(fileId, cancellationToken);
                return !deleted ? Results.NotFound() : Results.NoContent();
            })
            .WithName("DeleteFile")
            .WithDescription("Delete a file by ID");
    }
}
