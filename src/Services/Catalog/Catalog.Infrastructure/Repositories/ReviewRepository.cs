using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly CatalogDbContext _context;
    public ReviewRepository(CatalogDbContext context) =>
        _context = context;

    public async Task<IReadOnlyList<Review>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken) =>
        await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .ToListAsync(cancellationToken);
        
    public async Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken) =>
        await _context.Reviews
            .AnyAsync(r => r.StudentId == studentId && r.CourseId == courseId, cancellationToken);
            
    public async Task AddAsync(Review review, CancellationToken cancellationToken) =>
        await _context.Reviews.AddAsync(review, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await _context.SaveChangesAsync(cancellationToken);

}