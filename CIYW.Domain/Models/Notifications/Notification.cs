using CIYW.Const.Enums;

namespace CIYW.Domain.Models.Notifications;

public class Notification: BaseWithDateEntity
{
    public DateTime? Read { get; set; }
    public EntityTypeEnum EntityType { get; set; }
    public Guid EntityId { get; set; }
    public NotificationTypeEnum Type { get; set; }
    public Guid UserId { get; set; }
}