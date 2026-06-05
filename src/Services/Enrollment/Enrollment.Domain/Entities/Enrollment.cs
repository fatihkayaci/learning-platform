using Enrollment.Domain.Exceptions;

namespace Enrollment.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public int TotalLessonCount { get; private set; }

    private Enrollment() { }

    public static Enrollment Create(Guid studentId, Guid courseId, int totalLessonCount)
    {
        if (studentId == Guid.Empty)
            throw new BusinessException("StudentId cannot be empty.");
        if (courseId == Guid.Empty)
            throw new BusinessException("CourseId cannot be empty.");

        return new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            TotalLessonCount = totalLessonCount
        };
    }

    public void IncrementTotalLessonCount()
    {
        TotalLessonCount++;
    }
}
