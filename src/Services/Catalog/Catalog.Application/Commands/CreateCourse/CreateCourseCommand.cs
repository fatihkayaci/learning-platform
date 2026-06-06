using BuildingBlocks.Common.Idempotency;
using Catalog.Application.DTOs;

namespace Catalog.Application.Commands.CreateCourse;

public record CreateCourseCommand(string Name, string? Description, Guid CategoryId) : IIdempotentCommand<CourseDto>;