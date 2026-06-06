using BuildingBlocks.Common.Idempotency;

namespace Enrollment.Application.Commands.CompleteLesson;

public record CompleteLessonCommand(Guid CourseId, Guid LessonId) : IIdempotentCommand<Guid>;
