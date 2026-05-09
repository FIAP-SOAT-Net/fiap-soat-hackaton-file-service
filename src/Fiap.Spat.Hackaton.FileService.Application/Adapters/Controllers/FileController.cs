using Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers;

public class FileController(ILogger<FileController> logger, IUploadFileHandler uploadFileHandler) : IFileController
{
    public async Task<IResult> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        UploadFileCommand command = new(request.Name, request.Content, request.ContentType, request.Size);
        var response = await uploadFileHandler.Handle(command, cancellationToken);
        return response.IsSuccess ? TypedResults.Created() : TypedResults.BadRequest(response.Reasons);
    }
}
