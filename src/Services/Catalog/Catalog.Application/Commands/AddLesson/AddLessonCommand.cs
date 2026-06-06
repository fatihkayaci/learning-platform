using BuildingBlocks.Common.Idempotency;
using Catalog.Application.DTOs;

namespace Catalog.Application.Commands.AddLesson;

public record AddLessonCommand(string Title, string VideoUrl, int Order, Guid CourseId) : IIdempotentCommand<LessonDto>;