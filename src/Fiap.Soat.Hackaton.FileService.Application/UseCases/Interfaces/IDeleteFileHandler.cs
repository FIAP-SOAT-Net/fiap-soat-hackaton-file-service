using Fiap.Soat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IDeleteFileHandler
{
    Task<Response> HandleAsync(DeleteFileCommand request, CancellationToken cancellationToken);
}
