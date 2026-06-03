using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enrollment.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Domain.Entities.Enrollment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();
    }
}
