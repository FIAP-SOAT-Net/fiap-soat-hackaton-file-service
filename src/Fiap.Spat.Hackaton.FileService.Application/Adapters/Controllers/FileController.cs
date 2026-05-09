using Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;
using Microsoft.AspNetCore.Http;
namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers;

public class FileController(IUploadFileHandler uploadFileHandler, IGetFileHandler getFileHandler, IListFilesHandler listFilesHandler, IDeleteFileHandler deleteFileHandler) : IFileController
{
    public async Task<IResult> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        UploadFileCommand command = new(request.Name, request.Content, request.ContentType, request.Size);
        var response = await uploadFileHandler.Handle(command, cancellationToken);
        return response.IsSuccess ? TypedResults.Created() : TypedResults.BadRequest(response.Reasons);
    }

    public async Task<IResult> GetAsync(GetFileByIdQuery query, CancellationToken cancellationToken)
    {
        var response = await getFileHandler.HandleAsync(query, cancellationToken);
        return response.IsSuccess ? TypedResults.Ok(response.Data) : TypedResults.NotFound(response.Reasons);
    }

    public async Task<IResult> ListAsync(GetFilesQuery query, CancellationToken cancellationToken)
    {
        var response = await listFilesHandler.HandleAsync(query, cancellationToken);

        return response.IsSuccess
            ? TypedResults.Ok(response.Data.Select(f => new
            {
                f.Id,
                f.FileName,
                f.ContentType,
                f.Size,
                f.UploadedAt
            }))
            : TypedResults.BadRequest(response.Reasons);
    }

    public async Task<IResult> DeleteAsync(DeleteFileCommand command, CancellationToken cancellationToken)
    {
        var response = await deleteFileHandler.HandleAsync(command, cancellationToken);
        return response.IsSuccess ? TypedResults.NoContent() : TypedResults.NotFound(response.Reasons);
    }
}
