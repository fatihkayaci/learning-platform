using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDto> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        Course course = Course.Create(request.Name, request.InstructorId, request.CategoryId, request.Description);
        await _courseRepository.AddAsync(course, cancellationToken);
        await _courseRepository.SaveChangesAsync(cancellationToken);
        return new CourseDto(course.Id, course.Name, course.Description, course.InstructorId, course.CategoryId);
    }
}