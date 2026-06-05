namespace BuildingBlocks.Messaging.Events.Enrollment;

public record StudentEnrolledEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    DateTime EnrolledAt
);
