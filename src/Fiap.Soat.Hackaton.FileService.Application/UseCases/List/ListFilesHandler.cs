using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.List;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.UseCases.List;

public sealed class ListFilesHandler(IFileRepository fileRepository) : IListFilesHandler
{
    public async Task<Response<IEnumerable<FileDocument>>> HandleAsync(GetFilesQuery query, CancellationToken cancellationToken)
    {
        var files = await fileRepository.GetAllAsync(cancellationToken);
        return ResponseFactory.Ok<IEnumerable<FileDocument>>(files);
    }
}
