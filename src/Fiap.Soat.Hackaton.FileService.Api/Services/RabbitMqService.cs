// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
//
// namespace Fiap.Soat.Hackaton.FileService.Api.Services;
//
// /// <summary>
// /// Service for publishing messages to RabbitMQ
// /// </summary>
// public interface IMessagePublisher
// {
//     Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default) where T : class;
// }
//
// public class RabbitMqPublisher : IMessagePublisher
// {
//     private readonly IConnection _connection;
//     private readonly ILogger<RabbitMqPublisher> _logger;
//
//     public RabbitMqPublisher(IConnection connection, ILogger<RabbitMqPublisher> logger)
//     {
//         _connection = connection;
//         _logger = logger;
//     }
//
//     public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
//     {
//         try
//         {
//             using var channel = _connection.CreateModel();
//
//             // Declare exchange
//             channel.ExchangeDeclare(
//                 exchange: exchange,
//                 type: ExchangeType.Direct,
//                 durable: true,
//                 autoDelete: false,
//                 arguments: null);
//
//             // Serialize message
//             var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);
//             var body = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
//
//             // Publish message
//             var basicProperties = channel.CreateBasicProperties();
//             basicProperties.Persistent = true;
//             basicProperties.ContentType = "application/json";
//
//             channel.BasicPublish(
//                 exchange: exchange,
//                 routingKey: routingKey,
//                 mandatory: false,
//                 basicProperties: basicProperties,
//                 body: body);
//
//             _logger.LogInformation(
//                 "Message published successfully. Exchange: {Exchange}, RoutingKey: {RoutingKey}, Type: {MessageType}",
//                 exchange, routingKey, typeof(T).Name);
//
//             await Task.CompletedTask;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error publishing message to RabbitMQ. Exchange: {Exchange}, RoutingKey: {RoutingKey}",
//                 exchange, routingKey);
//             throw;
//         }
//     }
// }
//
// /// <summary>
// /// Service for consuming messages from RabbitMQ
// /// </summary>
// public interface IMessageConsumer
// {
//     void StartConsuming(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default);
// }
//
// public class RabbitMqConsumer : IMessageConsumer
// {
//     private readonly IConnection _connection;
//     private readonly ILogger<RabbitMqConsumer> _logger;
//
//     public RabbitMqConsumer(IConnection connection, ILogger<RabbitMqConsumer> logger)
//     {
//         _connection = connection;
//         _logger = logger;
//     }
//
//     public void StartConsuming(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default)
//     {
//         try
//         {
//             var channel = _connection.CreateModel();
//
//             // Declare queue
//             channel.QueueDeclare(
//                 queue: queue,
//                 durable: true,
//                 exclusive: false,
//                 autoDelete: false,
//                 arguments: null);
//
//             channel.BasicQos(0, 1, false); // Process one message at a time
//
//             var consumer = new AsyncEventingBasicConsumer(channel);
//
//             consumer.Received += async (model, ea) =>
//             {
//                 try
//                 {
//                     var message = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
//                     _logger.LogInformation("Message received from queue: {Queue}, Message: {Message}", queue, message);
//
//                     await messageHandler(message);
//
//                     channel.BasicAck(ea.DeliveryTag, false);
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, "Error processing message from queue: {Queue}", queue);
//                     channel.BasicNack(ea.DeliveryTag, false, true); // Requeue message
//                 }
//             };
//
//             channel.BasicConsume(
//                 queue: queue,
//                 autoAck: false,
//                 consumerTag: $"consumer-{queue}",
//                 noLocal: false,
//                 exclusive: false,
//                 arguments: null,
//                 consumer: consumer);
//
//             _logger.LogInformation("Started consuming messages from queue: {Queue}", queue);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error starting to consume messages from queue: {Queue}", queue);
//             throw;
//         }
//     }
// }
