using Enrollment.Application.Common.Interfaces;
using Enrollment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enrollment.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly EnrollmentDbContext _context;

    public EnrollmentRepository(EnrollmentDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Domain.Entities.Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
