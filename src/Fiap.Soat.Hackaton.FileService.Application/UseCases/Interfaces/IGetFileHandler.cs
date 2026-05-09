using Fiap.Soat.Hackaton.FileService.Application.UseCases.Get;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IGetFileHandler
{
    Task<Response<FileDocument>> HandleAsync(GetFileByIdQuery request, CancellationToken cancellationToken);
}
