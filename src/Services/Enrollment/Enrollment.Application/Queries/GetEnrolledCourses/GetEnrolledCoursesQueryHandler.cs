using Enrollment.Application.Common.Interfaces;
using Enrollment.Application.DTOs;
using MediatR;

namespace Enrollment.Application.Queries.GetEnrolledCourses;

public class GetEnrolledCoursesQueryHandler : IRequestHandler<GetEnrolledCoursesQuery, List<EnrolledCourseDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetEnrolledCoursesQueryHandler(
        IEnrollmentRepository enrollmentRepository,
        ILessonProgressRepository lessonProgressRepository,
        ICurrentUserService currentUserService)
    {
        _enrollmentRepository = enrollmentRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<EnrolledCourseDto>> Handle(GetEnrolledCoursesQuery request, CancellationToken cancellationToken)
    {
        Guid studentId = _currentUserService.UserId;

        List<Domain.Entities.Enrollment> enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);

        List<EnrolledCourseDto> result = new();

        foreach (Domain.Entities.Enrollment enrollment in enrollments)
        {
            int completedLessons = await _lessonProgressRepository.CountByEnrollmentIdAsync(enrollment.Id, cancellationToken);

            double progressPercentage = enrollment.TotalLessonCount == 0
                ? 0
                : Math.Round((double)completedLessons / enrollment.TotalLessonCount * 100, 2);

            result.Add(new EnrolledCourseDto(enrollment.Id, enrollment.CourseId, enrollment.CreatedAt, progressPercentage));
        }

        return result;
    }
}
