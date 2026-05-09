using Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IGetFileHandler
{
    Task<Response<FileDocument>> HandleAsync(GetFileByIdQuery request, CancellationToken cancellationToken);
}
