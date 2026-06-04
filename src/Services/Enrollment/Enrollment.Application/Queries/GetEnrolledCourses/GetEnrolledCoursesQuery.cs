using Enrollment.Application.DTOs;
using MediatR;

namespace Enrollment.Application.Queries.GetEnrolledCourses;

public record GetEnrolledCoursesQuery : IRequest<List<EnrolledCourseDto>>;
