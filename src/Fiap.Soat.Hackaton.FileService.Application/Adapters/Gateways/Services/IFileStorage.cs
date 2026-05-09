using Fiap.Soat.Hackaton.FileService.Application.Models;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Services;

public interface IFileStorage
{
    Task<Response<FileDetails>> UploadFileAsync(FileDocument file, CancellationToken cancellationToken = default);
}
