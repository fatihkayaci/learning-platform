using BuildingBlocks.Common.Behaviors;
using Enrollment.Application.Commands.EnrollInCourse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enrollment.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(EnrollInCourseCommand).Assembly);
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<EnrollInCourseCommand>();

        return services;
    }
}
