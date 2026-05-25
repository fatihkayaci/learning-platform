using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
namespace Catalog.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly CatalogDbContext _context;
    public LessonRepository(CatalogDbContext context) =>
        _context = context;

    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken) => 
        await _context.Lessons.AddAsync(lesson, cancellationToken);
        
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => 
        await _context.SaveChangesAsync(cancellationToken);

}