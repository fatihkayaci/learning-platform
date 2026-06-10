using Notification.Worker.Domain.Enums;

namespace Notification.Worker.Domain.Entities;

public class NotificationLog
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Email { get; private set; } = null!;
    public DateTime SentAt { get; private set; }
    public bool IsSuccessful { get; private set; }

    private NotificationLog() { }

    public static NotificationLog Create(Guid userId, NotificationType type, string email)
    {
        return new NotificationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Email = email,
            SentAt = DateTime.UtcNow,
            IsSuccessful = true
        };
    }
}
