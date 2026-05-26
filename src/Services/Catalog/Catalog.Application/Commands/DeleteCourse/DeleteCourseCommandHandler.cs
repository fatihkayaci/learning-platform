using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;

namespace Catalog.Application.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public DeleteCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.id, cancellationToken);
        if (course == null)
            throw new NotFoundException($"Course with id '{request.id}' not found.");
        
        await _courseRepository.DeleteAsync(request.id, cancellationToken);
    }
}