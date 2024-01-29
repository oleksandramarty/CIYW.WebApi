using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SignalRMessageTypeEnum
{
    [Description("MessageToAllActiveUsers")]MESSAGE_TO_ALL_ACTIVE_USERS = 1,
    [Description("MessageToUser")]MESSAGE_TO_USER = 2,
    [Description("AddToGroup")]ADD_TO_GROUP = 3,
    [Description("RemoveFromGroup")]REMOVE_FROM_GROUP = 4,
}