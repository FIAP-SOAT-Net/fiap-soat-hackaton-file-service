using Fiap.Spat.Hackaton.FileService.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Controllers.Interfaces;

public interface IFileController
{
    Task<IResult> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken);
}
