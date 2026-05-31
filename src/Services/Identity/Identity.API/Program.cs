using FluentValidation;
using Identity.API.Middleware;
using Identity.Application.Commands.RegisterUser;
using Identity.Application.Common.Interfaces;
using Identity.Infrastructure.Messaging;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommandHandler>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
