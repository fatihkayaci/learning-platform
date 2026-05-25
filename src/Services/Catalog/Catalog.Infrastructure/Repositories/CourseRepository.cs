using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Catalog.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CatalogDbContext _context;
    public CourseRepository(CatalogDbContext context) =>
        _context = context;

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)=>
        await _context.Courses
        .Include(c => c.Lessons)
        .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(Course course, CancellationToken cancellationToken) => 
        await _context.Courses.AddAsync(course, cancellationToken);

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Courses.ToListAsync(cancellationToken);
        
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => 
        await _context.SaveChangesAsync(cancellationToken);

}