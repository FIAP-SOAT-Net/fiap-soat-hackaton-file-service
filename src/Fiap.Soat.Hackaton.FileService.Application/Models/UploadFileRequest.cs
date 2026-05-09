namespace Fiap.Soat.Hackaton.FileService.Application.Models;

public record UploadFileRequest(string Name, byte[] Content, string ContentType, long Size);
