using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationTypeEnum
{
}