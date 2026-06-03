using MediatR;

namespace Enrollment.Application.Commands.EnrollInCourse;

public record EnrollInCourseCommand(Guid CourseId) : IRequest<Guid>;
