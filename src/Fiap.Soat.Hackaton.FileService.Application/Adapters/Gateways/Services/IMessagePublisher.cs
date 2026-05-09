namespace Fiap.Soat.Hackaton.FileService.Application.Adapters.Gateways.Services;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class;
}
