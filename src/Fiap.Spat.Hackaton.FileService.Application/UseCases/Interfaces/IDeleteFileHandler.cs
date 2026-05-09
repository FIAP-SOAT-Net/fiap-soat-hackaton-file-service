using Fiap.Spat.Hackaton.FileService.Application.UseCases.Delete;
using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IDeleteFileHandler
{
    Task<Response> HandleAsync(DeleteFileCommand request, CancellationToken cancellationToken);
}
