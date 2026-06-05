using MediatR;

namespace Enrollment.Application.Commands.CompleteLesson;

public record CompleteLessonCommand(Guid CourseId, Guid LessonId) : IRequest<Guid>;
