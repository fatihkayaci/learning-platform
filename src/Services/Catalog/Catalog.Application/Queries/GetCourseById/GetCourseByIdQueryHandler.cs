using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;

namespace Catalog.Application.Queries.GetCourseById;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDetailDto>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDetailDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (course == null)
            throw new NotFoundException($"Course with id '{request.Id}' not found.");

        return new CourseDetailDto(
            course.Id,
            course.Name,
            course.Description,
            course.InstructorId,
            course.CategoryId,
            course.Lessons.Select(l => new LessonDto(l.Id, l.Title, l.VideoUrl, l.Order)).ToList()
        );
    }
}
