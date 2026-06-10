namespace Catalog.Application.DTOs;

public record ReviewDto(Guid Id, Guid CourseId, Guid StudentId, int Rating, string Comment, DateTime CreatedAt);