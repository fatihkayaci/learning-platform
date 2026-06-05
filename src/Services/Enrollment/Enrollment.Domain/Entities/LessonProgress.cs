namespace Enrollment.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public Guid LessonId { get; private set; }

    private LessonProgress() { }

    public static LessonProgress Create(Guid enrollmentId, Guid lessonId)
    {
        return new LessonProgress
        {
            EnrollmentId = enrollmentId,
            LessonId = lessonId
        };
    }
}
