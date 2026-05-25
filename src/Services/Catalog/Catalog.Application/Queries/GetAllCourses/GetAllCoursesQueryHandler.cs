using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Queries.GetAllCourses;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, IReadOnlyList<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetAllCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IReadOnlyList<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Course> courses = await _courseRepository.GetAllAsync(cancellationToken);

        return courses.Select(c => new CourseDto(c.Id, c.Name, c.Description, c.InstructorId, c.CategoryId)).ToList();
    }
}
