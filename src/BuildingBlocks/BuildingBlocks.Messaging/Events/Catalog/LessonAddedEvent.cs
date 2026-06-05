namespace BuildingBlocks.Messaging.Events.Catalog;

public record LessonAddedEvent(
    Guid LessonId,
    Guid CourseId,
    DateTime AddedAt
);
