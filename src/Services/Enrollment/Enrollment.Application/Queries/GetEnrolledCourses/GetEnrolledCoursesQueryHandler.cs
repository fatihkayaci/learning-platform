using Enrollment.Application.Common.Interfaces;
using Enrollment.Application.DTOs;
using MediatR;

namespace Enrollment.Application.Queries.GetEnrolledCourses;

public class GetEnrolledCoursesQueryHandler : IRequestHandler<GetEnrolledCoursesQuery, List<EnrolledCourseDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetEnrolledCoursesQueryHandler(
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    {
        _enrollmentRepository = enrollmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<EnrolledCourseDto>> Handle(GetEnrolledCoursesQuery request, CancellationToken cancellationToken)
    {
        Guid studentId = _currentUserService.UserId;

        List<Domain.Entities.Enrollment> enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);

        return enrollments
            .Select(e => new EnrolledCourseDto(e.Id, e.CourseId, e.CreatedAt))
            .ToList();
    }
}
