namespace Fiap.Spat.Hackaton.FileService.Application.UseCases.Upload;

public record UploadFileCommand(string Name, byte[] Content, string ContentType, long Size);
