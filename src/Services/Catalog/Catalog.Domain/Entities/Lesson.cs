namespace Catalog.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string VideoUrl{ get; private set; } = null!;
    public int Order{ get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    public Lesson(){}

    public static Lesson Create(string title, string videoUrl, int order, Guid courseId)
    {
        return new Lesson
        {
            Title = title,
            VideoUrl = videoUrl,
            Order = order,
            CourseId = courseId
        }; 
    }
}