using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fiap.Soat.Hackaton.FileService.Integration.Tests;

public sealed class FileServiceApiFactory : WebApplicationFactory<Program>
{
    private readonly FakeFileRepository _fileRepository = new();
    private readonly FakeFileStorage _fileStorage = new();
    private readonly FakeMessagePublisher _messagePublisher = new();

    public FakeFileRepository FileRepository => _fileRepository;
    public FakeFileStorage FileStorage => _fileStorage;
    public FakeMessagePublisher MessagePublisher => _messagePublisher;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MongoDB"] = "mongodb://localhost:27017",
                ["MongoDb:Database"] = "fileservice-tests",
                ["RabbitMq:Host"] = "localhost",
                ["RabbitMq:Port"] = "5672",
                ["RabbitMq:UserName"] = "guest",
                ["RabbitMq:Password"] = "guest",
                ["RabbitMq:ExchangeName"] = "fileservice.tests.exchange",
                ["RabbitMq:QueueName"] = "fileservice.tests.queue",
                ["S3:ServiceUrl"] = "http://localhost:9000",
                ["S3:ForcePathStyle"] = "true",
                ["S3:Region"] = "us-east-1",
                ["S3:AccessKey"] = "test",
                ["S3:SecretKey"] = "test"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IFileRepository>();
            services.RemoveAll<IFileStorage>();
            services.RemoveAll<IMessagePublisher>();

            services.AddSingleton<IFileRepository>(_ => _fileRepository);
            services.AddSingleton<IFileStorage>(_ => _fileStorage);
            services.AddSingleton<IMessagePublisher>(_ => _messagePublisher);
        });
    }
}
