using Enrollment.Application.Common.Interfaces;
using Enrollment.Domain.Exceptions;
using MediatR;

namespace Enrollment.Application.Commands.EnrollInCourse;

public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, Guid>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseService _courseService;
    private readonly ICurrentUserService _currentUserService;

    public EnrollInCourseCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseService courseService,
        ICurrentUserService currentUserService)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseService = courseService;
        _currentUserService = currentUserService;
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

        Domain.Entities.Enrollment enrollment = Domain.Entities.Enrollment.Create(studentId, request.CourseId);

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        return enrollment.Id;
    }
}
