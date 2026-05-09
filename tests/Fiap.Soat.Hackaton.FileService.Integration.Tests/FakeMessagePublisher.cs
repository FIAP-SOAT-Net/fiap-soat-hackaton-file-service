using Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Services;

namespace Fiap.Soat.Hackaton.FileService.Integration.Tests;

public sealed class FakeMessagePublisher : IMessagePublisher
{
    public List<(string Exchange, string RoutingKey, object Message)> PublishedMessages { get; } = [];

    public Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        PublishedMessages.Add((exchange, routingKey, message));
        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class
        => PublishAsync("default", routingKey, message, cancellationToken);
}
