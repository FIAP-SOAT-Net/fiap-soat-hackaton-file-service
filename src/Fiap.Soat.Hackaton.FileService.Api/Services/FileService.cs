using Amazon.S3;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Fiap.Soat.Hackaton.FileService.Api.Services;

/// <summary>
/// Service for managing files in MongoDB
/// </summary>
public interface IFileService
{
    Task<string> SaveFileAsync(FileDto file, CancellationToken cancellationToken = default);
    Task<FileDocument?> GetFileAsync(string fileId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileDocument>> GetAllFilesAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);
}

public class FileService(IMongoDatabase database, IAmazonS3 amazonS3, IConfiguration configuration, ILogger<FileService> logger) : IFileService
{
    private readonly IMongoCollection<FileDocument> _fileCollection = database.GetCollection<FileDocument>("files");
    private readonly string _bucketName = configuration["S3:BucketName"] ?? "fileservice-bucket";

    public async Task<string> SaveFileAsync(FileDto file, CancellationToken cancellationToken = default)
    {
        if (file.Size == 0) throw new ArgumentException("File is empty", nameof(file));

        await amazonS3.EnsureBucketExistsAsync(_bucketName);
        var fileDocument = file.ToEntity();

        // Insert into S3
        await amazonS3.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
        {
            BucketName = _bucketName, // TODO: Need to add on RabbitMQ message
            Key = $"uploads/{fileDocument.Id}", // TODO: Need to add on RabbitMQ message
            InputStream = new MemoryStream(fileDocument.Content),
            ContentType = file.ContentType
        }, cancellationToken);

        // Insert into MongoDB
        await _fileCollection.InsertOneAsync(fileDocument, cancellationToken: cancellationToken);

        logger.LogInformation("File saved   successfully. Id: {FileId}, Name: {FileName}", fileDocument.Id, fileDocument.FileName);

        return fileDocument.Id.ToString();
    }

    public async Task<FileDocument?> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(fileId, out var objectId))
            return null;

        var filter = Builders<FileDocument>.Filter.Eq(f => f.Id, objectId);
        return await _fileCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<FileDocument>> GetAllFilesAsync(CancellationToken cancellationToken = default)
    {
        return await _fileCollection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(fileId, out var objectId))
            return false;

        var filter = Builders<FileDocument>.Filter.Eq(f => f.Id, objectId);
        var result = await _fileCollection.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        if (result.DeletedCount > 0)
            logger.LogInformation("File deleted successfully. Id: {FileId}", fileId);

        return result.DeletedCount > 0;
    }
}

/// <summary>
/// MongoDB document model for files
/// </summary>
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

public static class FileDocumentExtensions
{
    public static FileDocument ToEntity(this FileDto dto) => FileDocument.Create(dto.Name, dto.Content, dto.ContentType, dto.Size);
}

public record FileDto(string Name, byte[] Content, string ContentType, long Size);
