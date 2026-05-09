using Fiap.Spat.Hackaton.FileService.Application.Adapters.Gateways.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Services;

public sealed class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private bool _disposed;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = factory.CreateConnectionAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        ).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        return PublishAsync(_settings.ExchangeName, routingKey, message, cancellationToken);
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqPublisher));

        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken
        );
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _channel?.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        _channel?.Dispose();
        _connection?.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        _connection?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
