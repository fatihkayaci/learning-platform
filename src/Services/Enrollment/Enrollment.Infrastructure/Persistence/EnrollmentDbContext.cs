using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Enrollment.Infrastructure.Persistence;

public class EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Enrollment> Enrollments => Set<Domain.Entities.Enrollment>();
    public DbSet<Domain.Entities.LessonProgress> LessonProgresses => Set<Domain.Entities.LessonProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
