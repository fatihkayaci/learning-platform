using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;


namespace Catalog.Application.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCourseCommandHandler(ICourseRepository courseRepository, ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.id, cancellationToken);
        if (course == null)
            throw new NotFoundException($"Course with id '{request.id}' not found.");
        if (course.InstructorId != _currentUserService.UserId)
            throw new BusinessException("You are not the owner of this course.");

        await _courseRepository.DeleteAsync(request.id, cancellationToken);
    }
}