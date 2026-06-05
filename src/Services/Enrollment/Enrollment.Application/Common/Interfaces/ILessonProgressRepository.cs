using Enrollment.Domain.Entities;

namespace Enrollment.Application.Common.Interfaces;

public interface ILessonProgressRepository
{
    Task AddAsync(LessonProgress lessonProgress, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
