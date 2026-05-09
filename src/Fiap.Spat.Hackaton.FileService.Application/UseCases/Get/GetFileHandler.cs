using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Get;

public sealed class GetFileHandler(IFileRepository fileRepository) : IGetFileHandler
{
    public async Task<Response<FileDocument>> HandleAsync(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        var file = await fileRepository.GetAsync(request.Id, cancellationToken);
        return file != null ? ResponseFactory.Ok(file) : ResponseFactory.Fail<FileDocument>("File not found.");
    }
}
