using Fiap.Soat.Hackaton.FileService.Infrastructure.Services;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Tests.Services;

public class RabbitMqPublisherTests
{
    [Fact(Skip = "TODO: Revisit RabbitMqPublisher tests when constructor supports injected connection/channel abstractions for unit testing.")]
    public void Constructor_ShouldDeclareExchange()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "TODO: Revisit RabbitMqPublisher tests when constructor supports injected connection/channel abstractions for unit testing.")]
    public void PublishAsync_WithRoutingKey_ShouldUseSettingsExchange()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "TODO: Revisit RabbitMqPublisher tests when constructor supports injected connection/channel abstractions for unit testing.")]
    public void PublishAsync_WithExplicitExchange_ShouldPublishToGivenExchange()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "TODO: Revisit RabbitMqPublisher tests when constructor supports injected connection/channel abstractions for unit testing.")]
    public void PublishAsync_ShouldThrow_WhenDisposed()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "TODO: Revisit RabbitMqPublisher tests when constructor supports injected connection/channel abstractions for unit testing.")]
    public void Dispose_ShouldCloseAndDisposeResources_OnlyOnce()
    {
        // Arrange

        // Act

        // Assert
    }
}
