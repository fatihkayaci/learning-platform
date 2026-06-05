using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging.Events.Enrollment;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Catalog.Infrastructure.Messaging;

public class StudentEnrolledConsumer : BackgroundService
{
    private readonly ILogger<StudentEnrolledConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public StudentEnrolledConsumer(
        ILogger<StudentEnrolledConsumer> logger,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
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

        string exchangeName = nameof(StudentEnrolledEvent);
        string queueName = "catalog.student-enrolled";

        await _channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken
        );

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new(_channel!);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            string json = Encoding.UTF8.GetString(ea.Body.ToArray());
            StudentEnrolledEvent? @event = JsonSerializer.Deserialize<StudentEnrolledEvent>(json);

            if (@event is not null)
            {
                _logger.LogInformation("[Catalog] StudentEnrolledEvent received — EnrollmentId: {EnrollmentId}, CourseId: {CourseId}",
                    @event.EnrollmentId, @event.CourseId);

                using IServiceScope scope = _scopeFactory.CreateScope();
                ICourseRepository courseRepository = scope.ServiceProvider.GetRequiredService<ICourseRepository>();

                Course? course = await courseRepository.GetByIdAsync(@event.CourseId, stoppingToken);
                if (course is not null)
                {
                    course.IncrementEnrollmentCount();
                    await courseRepository.SaveChangesAsync(stoppingToken);
                }
            }

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        await _channel!.BasicConsumeAsync(
            queue: "catalog.student-enrolled",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
