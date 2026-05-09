using MongoDB.Bson;

namespace Fiap.Spat.Hackaton.FileService.Domain.Entities;

public class FileDocument
{
    [MongoDB.Bson.Serialization.Attributes.BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    public byte[] Content { get; set; } = [];

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public static FileDocument Create(string fileName, byte[] content, string contentType, long size)
    {
        return new FileDocument
        {
            FileName = fileName,
            Content = content,
            ContentType = contentType,
            Size = size,
            UploadedAt = DateTime.UtcNow
        };
    }
}
