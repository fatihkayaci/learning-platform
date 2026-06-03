namespace Enrollment.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }

    private Enrollment() { }

    public static Enrollment Create(Guid studentId, Guid courseId)
    {
        return new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId
        };
    }
}
