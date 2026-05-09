using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Repositories;

public class FileRepository(IMongoDatabase database) : IFileRepository
{
    private readonly IMongoCollection<FileDocument> _fileCollection = database.GetCollection<FileDocument>("files");

    public async Task InsertAsync(FileDocument fileDocument, CancellationToken cancellationToken = default)
    {
        await _fileCollection.InsertOneAsync(fileDocument, cancellationToken: cancellationToken);
    }

    public async Task<FileDocument?> GetAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(fileId, out var objectId)) return null;

        var filter = Builders<FileDocument>.Filter.Eq(f => f.Id, objectId);
        return await _fileCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<FileDocument>> GetAllAsync(CancellationToken cancellationToken = default) => _fileCollection.Find(_ => true).ToListAsync(cancellationToken);

    public async Task<bool> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(fileId, out var objectId)) return false;

        var filter = Builders<FileDocument>.Filter.Eq(f => f.Id, objectId);
        var result = await _fileCollection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}
