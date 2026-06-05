using Identity.Application.Extensions;
using Identity.Infrastructure.Persistence.Seeders;
using Identity.API.Middleware;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using Identity.Application.Common.Interfaces;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IEventPublisher>(_ =>
    RabbitMqEventPublisher.CreateAsync(
        hostName: builder.Configuration["RabbitMQ:Host"]!,
        username: builder.Configuration["RabbitMQ:Username"]!,
        password: builder.Configuration["RabbitMQ:Password"]!
    ).GetAwaiter().GetResult()
);

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddApplicationServices();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (IServiceScope scope = app.Services.CreateScope())
{
    IdentityDbContext db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await db.Database.MigrateAsync();
    await IdentitySeeder.SeedAsync(db);
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
