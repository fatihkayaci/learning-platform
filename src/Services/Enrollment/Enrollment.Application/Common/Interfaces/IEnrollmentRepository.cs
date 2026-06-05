namespace Enrollment.Application.Common.Interfaces;

public interface IEnrollmentRepository
{
    Task AddAsync(Domain.Entities.Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
