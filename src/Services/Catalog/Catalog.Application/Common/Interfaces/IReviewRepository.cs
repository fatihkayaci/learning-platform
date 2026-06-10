using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Interfaces;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
    Task AddAsync(Review review, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}