using CIYW.Const.Enums;

namespace CIYW.SignalR.Models;

public class MessageHubModel
{
    public MessageHubModel(SignalRMessageTypeEnum signalRMessageType, string message)
    {
        Message = message;
        SignalRMessageType = signalRMessageType;
    }

    public string Message { get; set; }
    public SignalRMessageTypeEnum SignalRMessageType { get; set; }

    public static MessageHubModel ToAllUsers(string message)
    {
        return new MessageHubModel(SignalRMessageTypeEnum.MESSAGE_TO_ALL_ACTIVE_USERS, message);
    }
    public static MessageHubModel ToUser(string message)
    {
        return new MessageHubModel(SignalRMessageTypeEnum.MESSAGE_TO_USER, message);
    }
}