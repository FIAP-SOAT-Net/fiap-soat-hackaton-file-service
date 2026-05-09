using Fiap.Spat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IUploadFileHandler
{
    Task<Response> Handle(UploadFileCommand request, CancellationToken cancellationToken);
}
