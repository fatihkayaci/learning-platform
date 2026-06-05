namespace Enrollment.Application.DTOs;

public record EnrolledCourseDto(Guid EnrollmentId, Guid CourseId, DateTime EnrolledAt, double ProgressPercentage);
