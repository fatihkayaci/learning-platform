using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;

namespace Catalog.Application.Commands.UpdateCourse;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDto> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.id, cancellationToken);
        if (course == null)
            throw new NotFoundException($"Course with id '{request.id}' not found.");
        course.Update(request.Name, request.Description, request.CategoryId);
        await _courseRepository.SaveChangesAsync(cancellationToken);
        return new CourseDto(course.Id, course.Name, course.Description, course.InstructorId, course.CategoryId);
    }
}