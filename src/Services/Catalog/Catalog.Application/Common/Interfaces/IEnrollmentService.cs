namespace Catalog.Application.Common.Interfaces;
public interface IEnrollmentService
{
    Task<bool> IsEnrolledAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
}