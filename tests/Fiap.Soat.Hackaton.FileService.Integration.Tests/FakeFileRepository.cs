using System.Collections.Concurrent;
using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Spat.Hackaton.FileService.Domain.Entities;
namespace Fiap.Soat.Hackaton.FileService.Integration.Tests;

public sealed class FakeFileRepository : IFileRepository
{
    private readonly ConcurrentDictionary<string, FileDocument> _files = new();

    public IReadOnlyCollection<FileDocument> InsertedFiles => _files.Values.ToArray();

    public Task InsertAsync(FileDocument fileDocument, CancellationToken cancellationToken = default)
    {
        _files[fileDocument.Id.ToString()] = fileDocument;
        return Task.CompletedTask;
    }

    public Task<FileDocument?> GetAsync(string fileId, CancellationToken cancellationToken = default)
        => Task.FromResult(_files.TryGetValue(fileId, out var fileDocument) ? fileDocument : null);

    public Task<List<FileDocument>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_files.Values.ToList());

    public Task<bool> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
        => Task.FromResult(_files.TryRemove(fileId, out _));
}
