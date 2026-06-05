using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Catalog;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;

namespace Catalog.Application.Commands.AddLesson;

public class AddLessonCommandHandler : IRequestHandler<AddLessonCommand, LessonDto>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventPublisher _eventPublisher;

    public AddLessonCommandHandler(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService,
        IEventPublisher eventPublisher)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
        _eventPublisher = eventPublisher;
    }

    public async Task<LessonDto> Handle(AddLessonCommand request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new NotFoundException($"Course with id '{request.CourseId}' not found.");
        if (course.InstructorId != _currentUserService.UserId)
            throw new BusinessException("You are not the owner of this course.");

        Lesson lesson = Lesson.Create(request.Title, request.VideoUrl, request.Order, request.CourseId);
        await _lessonRepository.AddAsync(lesson, cancellationToken);
        await _lessonRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new LessonAddedEvent(lesson.Id, lesson.CourseId, DateTime.UtcNow), cancellationToken);

        return new LessonDto(lesson.Id, lesson.Title, lesson.VideoUrl, lesson.Order);
    }
}