using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging.Events.Enrollment;
using Notification.Worker.Domain.Entities;
using Notification.Worker.Domain.Enums;
using Notification.Worker.Infrastructure.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notification.Worker.Consumers;

public class StudentEnrolledConsumer : BackgroundService
{
    private readonly ILogger<StudentEnrolledConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public StudentEnrolledConsumer(
        ILogger<StudentEnrolledConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = _configuration["RabbitMQ:Host"]!,
            UserName = _configuration["RabbitMQ:Username"]!,
            Password = _configuration["RabbitMQ:Password"]!
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: nameof(StudentEnrolledEvent),
            type: ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: "notification.student-enrolled",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: "notification.student-enrolled",
            exchange: nameof(StudentEnrolledEvent),
            routingKey: string.Empty,
            cancellationToken: cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new(_channel!);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            string json = Encoding.UTF8.GetString(ea.Body.ToArray());
            StudentEnrolledEvent? @event = JsonSerializer.Deserialize<StudentEnrolledEvent>(json);

            if (@event is null)
            {
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                NotificationDbContext db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                NotificationLog log = NotificationLog.Create(@event.StudentId, NotificationType.EnrollmentConfirmation, string.Empty);
                db.NotificationLogs.Add(log);
                await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Enrollment confirmation sent to student {StudentId} for course {CourseId}",
                    @event.StudentId, @event.CourseId);

                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process StudentEnrolledEvent for student {StudentId}", @event.StudentId);
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: "notification.student-enrolled",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
