using Fiap.Soat.Hackaton.FileService.Application.UseCases.Upload;
using Fiap.Soat.Hackaton.FileService.Domain.Shared;

namespace Fiap.Soat.Hackaton.FileService.Application.UseCases.Interfaces;

public interface IUploadFileHandler
{
    Task<Response> Handle(UploadFileCommand request, CancellationToken cancellationToken);
}
