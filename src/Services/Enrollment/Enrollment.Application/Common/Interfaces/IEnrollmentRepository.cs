namespace Enrollment.Application.Common.Interfaces;

public interface IEnrollmentRepository
{
    Task AddAsync(Domain.Entities.Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
