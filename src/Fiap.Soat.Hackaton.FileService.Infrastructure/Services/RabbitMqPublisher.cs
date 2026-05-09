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
    private readonly IModel _channel;
    private bool _disposed;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;

        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );
    }

    public Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        return PublishAsync(_settings.ExchangeName, routingKey, message, cancellationToken);
    }

    public Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqPublisher));

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.Persistent = true;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        _channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body
        );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
