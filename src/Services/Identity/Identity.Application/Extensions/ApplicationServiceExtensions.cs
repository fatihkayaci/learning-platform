using BuildingBlocks.Common.Behaviors;
using FluentValidation;
using Identity.Application.Commands.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<RegisterUserCommandHandler>();

        return services;
    }
}
