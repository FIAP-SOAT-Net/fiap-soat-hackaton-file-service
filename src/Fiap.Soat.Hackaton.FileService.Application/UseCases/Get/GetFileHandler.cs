using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.FileService.Application.UseCases.Interfaces;
using Fiap.Soat.Hackaton.FileService.Domain.Entities;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.UseCases.Get;

public sealed class GetFileHandler(IFileRepository fileRepository) : IGetFileHandler
{
    public async Task<Response<FileDocument>> HandleAsync(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        var file = await fileRepository.GetAsync(request.Id, cancellationToken);
        return file != null ? ResponseFactory.Ok(file) : ResponseFactory.Fail<FileDocument>("File not found.");
    }
}
