namespace Catalog.Application.DTOs;

public record LessonDto(Guid Id, string Title, string VideoUrl, int Order);