using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Enrollment;
using Enrollment.Application.Common.Interfaces;
using Enrollment.Domain.Exceptions;
using MediatR;

namespace Enrollment.Application.Commands.EnrollInCourse;

public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, Guid>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseService _courseService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventPublisher _eventPublisher;

    public EnrollInCourseCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseService courseService,
        ICurrentUserService currentUserService,
        IEventPublisher eventPublisher)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseService = courseService;
        _currentUserService = currentUserService;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken)
    {
        bool courseExists = await _courseService.ExistsAsync(request.CourseId, cancellationToken);
        if (!courseExists)
            throw new NotFoundException($"Course {request.CourseId} not found.");

        Guid studentId = _currentUserService.UserId;

        bool alreadyEnrolled = await _enrollmentRepository.ExistsAsync(studentId, request.CourseId, cancellationToken);
        if (alreadyEnrolled)
            throw new BusinessException("You are already enrolled in this course.");

        int totalLessonCount = await _courseService.GetLessonCountAsync(request.CourseId, cancellationToken);

        Domain.Entities.Enrollment enrollment = Domain.Entities.Enrollment.Create(studentId, request.CourseId, totalLessonCount);

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(
            new StudentEnrolledEvent(enrollment.Id, studentId, request.CourseId, DateTime.UtcNow),
            cancellationToken);

        return enrollment.Id;
    }
}
