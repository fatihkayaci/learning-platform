using Enrollment.Domain.Exceptions;

namespace Enrollment.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public Guid LessonId { get; private set; }

    private LessonProgress() { }

    public static LessonProgress Create(Guid enrollmentId, Guid lessonId)
    {
        if (enrollmentId == Guid.Empty)
            throw new BusinessException("EnrollmentId cannot be empty.");
        if (lessonId == Guid.Empty)
            throw new BusinessException("LessonId cannot be empty.");

        return new LessonProgress
        {
            EnrollmentId = enrollmentId,
            LessonId = lessonId
        };
    }
}
