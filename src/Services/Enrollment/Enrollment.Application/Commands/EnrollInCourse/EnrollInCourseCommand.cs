using BuildingBlocks.Common.Idempotency;

namespace Enrollment.Application.Commands.EnrollInCourse;

public record EnrollInCourseCommand(Guid CourseId) : IIdempotentCommand<Guid>;
