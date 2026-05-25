namespace Catalog.Application.DTOs;

public record CourseDto(Guid Id, string Name, string Description, Guid InstructorId, Guid CategoryId);
