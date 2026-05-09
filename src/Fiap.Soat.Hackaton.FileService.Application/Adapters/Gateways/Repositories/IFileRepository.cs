using Fiap.Soat.Hackaton.FileService.Domain.Entities;

namespace Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;

public interface IFileRepository
{
    Task InsertAsync(FileDocument fileDocument, CancellationToken cancellationToken = default);
    Task<FileDocument?> GetAsync(string fileId, CancellationToken cancellationToken = default);
    Task<List<FileDocument>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileId, CancellationToken cancellationToken = default);
}
