using System.Text;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.RabbitMQ;
using Enrollment.API.Middleware;
using Enrollment.Application.Extensions;
using Enrollment.Application.Common.Interfaces;
using Enrollment.Infrastructure.Messaging;
using Enrollment.Infrastructure.Persistence;
using Enrollment.Infrastructure.Repositories;
using Enrollment.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogLogging();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<EnrollmentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();

builder.Services.AddHttpClient<ICourseService, CourseService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CatalogUrl"]!);
});

builder.Services.AddApplicationServices();

builder.Services.AddHostedService<LessonAddedConsumer>();

builder.Services.AddSingleton<IEventPublisher>(_ =>
    RabbitMqEventPublisher.CreateAsync(
        builder.Configuration["RabbitMQ:Host"]!,
        builder.Configuration["RabbitMQ:Username"]!,
        builder.Configuration["RabbitMQ:Password"]!
    ).GetAwaiter().GetResult()
);

builder.Services.AddControllers();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    EnrollmentDbContext db = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
