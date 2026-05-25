namespace Catalog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ICollection<Course> Courses { get; private set; } = new List<Course>();
    
    public Category() { }
    
    public static Category Create(string name)
    {
        return new Category { Name = name };
    }

}