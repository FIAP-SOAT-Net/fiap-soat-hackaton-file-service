using Microsoft.AspNetCore.Http;

namespace Fiap.Spat.Hackaton.FileService.Application.UseCases;

public record UploadFileCommand(string Name, byte[] Content, string ContentType, long Size);
