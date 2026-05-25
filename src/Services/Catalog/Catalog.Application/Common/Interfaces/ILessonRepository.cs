
using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Interfaces;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson , CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}