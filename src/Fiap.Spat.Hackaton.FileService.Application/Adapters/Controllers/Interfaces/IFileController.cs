using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Microsoft.AspNetCore.Http;

namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;

public interface IFileController
{
    Task<IResult> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken);
    Task<IResult> GetAsync(GetFileByIdQuery query, CancellationToken cancellationToken);
    Task<IResult> ListAsync(GetFilesQuery query, CancellationToken cancellationToken);
    Task<IResult> DeleteAsync(DeleteFileCommand command, CancellationToken cancellationToken);
}
