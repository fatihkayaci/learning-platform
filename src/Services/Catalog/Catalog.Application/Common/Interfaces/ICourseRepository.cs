
using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Course course, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Course course, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}