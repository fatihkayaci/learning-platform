using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Commands.CreateCourse;

public record CreateCourseCommand(string Name, string? Description, Guid InstructorId, Guid CategoryId) : IRequest<CourseDto>;