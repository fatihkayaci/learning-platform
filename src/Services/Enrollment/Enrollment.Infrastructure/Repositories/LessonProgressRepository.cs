using Enrollment.Application.Common.Interfaces;
using Enrollment.Domain.Entities;
using Enrollment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enrollment.Infrastructure.Repositories;

public class LessonProgressRepository : ILessonProgressRepository
{
    private readonly EnrollmentDbContext _context;

    public LessonProgressRepository(EnrollmentDbContext context) =>
        _context = context;

    public async Task AddAsync(LessonProgress lessonProgress, CancellationToken cancellationToken = default) =>
        await _context.LessonProgresses.AddAsync(lessonProgress, cancellationToken);

    public async Task<bool> ExistsAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default) =>
        await _context.LessonProgresses
            .AnyAsync(lp => lp.EnrollmentId == enrollmentId && lp.LessonId == lessonId, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
