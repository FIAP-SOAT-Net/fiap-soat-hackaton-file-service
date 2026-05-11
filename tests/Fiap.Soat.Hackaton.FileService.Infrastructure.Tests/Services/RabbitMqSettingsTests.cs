using Fiap.Soat.Hackaton.FileService.Infrastructure.Services;
using FluentAssertions;

namespace Fiap.Soat.Hackaton.FileService.Infrastructure.Tests.Services;

public class RabbitMqSettingsTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange

        // Act
        var settings = new RabbitMqSettings();

        // Assert
        settings.Host.Should().Be("localhost");
        settings.Port.Should().Be(5672);
        settings.User.Should().Be("guest");
        settings.Password.Should().Be("guest");
        settings.ExchangeName.Should().Be("fileservice.events.exchange");
        settings.QueueName.Should().Be("fileservice.events");
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var settings = new RabbitMqSettings();

        // Act
        settings.Host = "rabbit-host";
        settings.Port = 5673;
        settings.User = "user";
        settings.Password = "pass";
        settings.ExchangeName = "exchange";
        settings.QueueName = "queue";

        // Assert
        settings.Host.Should().Be("rabbit-host");
        settings.Port.Should().Be(5673);
        settings.User.Should().Be("user");
        settings.Password.Should().Be("pass");
        settings.ExchangeName.Should().Be("exchange");
        settings.QueueName.Should().Be("queue");
    }
}
