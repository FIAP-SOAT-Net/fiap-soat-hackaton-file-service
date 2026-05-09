using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.List;

public sealed class ListFilesHandler(IFileRepository fileRepository) : IListFilesHandler
{
    public async Task<Response<IEnumerable<FileDocument>>> HandleAsync(GetFilesQuery query, CancellationToken cancellationToken)
    {
        var files = await fileRepository.GetAllAsync(cancellationToken);
        return ResponseFactory.Ok<IEnumerable<FileDocument>>(files);
    }
}
