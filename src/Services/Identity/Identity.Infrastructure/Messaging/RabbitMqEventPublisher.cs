using System.Text;
using System.Text.Json;
using Identity.Application.Common.Interfaces;
using RabbitMQ.Client;

namespace Identity.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private RabbitMqEventPublisher(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqEventPublisher> CreateAsync(string hostName, string username, string password)
    {
        ConnectionFactory factory = new()
        {
            HostName = hostName,
            UserName = username,
            Password = password
        };

        IConnection connection = await factory.CreateConnectionAsync();
        IChannel channel = await connection.CreateChannelAsync();

        return new RabbitMqEventPublisher(connection, channel);
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        string exchangeName = typeof(T).Name;

        await _channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken
        );

        string json = JsonSerializer.Serialize(@event);
        byte[] body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: string.Empty,
            body: body,
            cancellationToken: cancellationToken
        );
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
