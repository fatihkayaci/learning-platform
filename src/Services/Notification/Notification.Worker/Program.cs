using Microsoft.EntityFrameworkCore;
using Notification.Worker.Consumers;
using Notification.Worker.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<UserRegisteredConsumer>();

var host = builder.Build();

using (IServiceScope scope = host.Services.CreateScope())
{
    NotificationDbContext db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    await db.Database.MigrateAsync();
}

host.Run();
