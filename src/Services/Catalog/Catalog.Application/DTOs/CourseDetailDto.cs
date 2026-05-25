namespace Catalog.Application.DTOs;

public record CourseDetailDto(Guid Id, string Name, string Description, Guid InstructorId, Guid CategoryId, IReadOnlyList<LessonDto> Lessons);