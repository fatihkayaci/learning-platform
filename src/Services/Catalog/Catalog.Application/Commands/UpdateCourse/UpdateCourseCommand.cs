using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Commands.UpdateCourse;

public record UpdateCourseCommand(Guid id, string Name, string? Description, Guid CategoryId) : IRequest<CourseDto>;