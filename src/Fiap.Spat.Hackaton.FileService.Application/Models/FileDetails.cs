namespace Fiap.Spat.Hackaton.FileService.Application.Models;

public record FileDetails(string Key, string BucketName, string FileName, byte[] Content, string ContentType, long Size, DateTime UploadedAt);
