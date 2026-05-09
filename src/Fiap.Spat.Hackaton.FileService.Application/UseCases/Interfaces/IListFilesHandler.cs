using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IListFilesHandler
{
    Task<Response<IEnumerable<FileDocument>>> HandleAsync(GetFilesQuery query, CancellationToken cancellationToken);
}
