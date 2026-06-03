namespace Enrollment.Application.Common.Interfaces;

public interface ICourseService
{
    Task<bool> ExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
}
