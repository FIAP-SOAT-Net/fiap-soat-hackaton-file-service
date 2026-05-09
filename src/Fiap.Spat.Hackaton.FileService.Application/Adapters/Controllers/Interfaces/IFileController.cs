using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Microsoft.AspNetCore.Http;

namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;

public interface IFileController
{
    Task<IResult> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken);
    Task<IResult> GetAsync(GetFileByIdQuery query, CancellationToken cancellationToken);
}
