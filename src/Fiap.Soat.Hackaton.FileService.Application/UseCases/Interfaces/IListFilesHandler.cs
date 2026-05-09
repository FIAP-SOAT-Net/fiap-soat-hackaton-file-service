using Fiap.Soat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IListFilesHandler
{
    Task<Response<IEnumerable<FileDocument>>> HandleAsync(GetFilesQuery query, CancellationToken cancellationToken);
}
