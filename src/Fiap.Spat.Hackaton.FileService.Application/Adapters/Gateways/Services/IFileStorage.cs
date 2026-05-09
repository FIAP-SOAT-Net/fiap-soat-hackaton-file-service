using Fiap.Spat.Hackaton.FileService.Application.Models;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;

public interface IFileStorage
{
    Task<Response<FileDetails>> UploadFileAsync(FileDocument file, CancellationToken cancellationToken = default);
}
