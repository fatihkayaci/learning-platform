using MediatR;

namespace Enrollment.Application.Queries.CheckEnrollment;

public record CheckEnrollmentQuery(Guid StudentId, Guid CourseId) : IRequest<bool>;
