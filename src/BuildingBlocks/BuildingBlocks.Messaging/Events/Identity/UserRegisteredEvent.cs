namespace BuildingBlocks.Messaging.Events.Identity;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string Role,
    DateTime RegisteredAt
);
