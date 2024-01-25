using CIYW.Domain;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.SignalR.Models;
using Microsoft.AspNetCore.SignalR;

namespace CIYW.SignalR;

public class MessageHub: CommonHub
{
    public MessageHub(
        ICurrentUserProvider currentUserProvider, 
        IGenericRepository<ActiveUser> activeUserRepository) : base(currentUserProvider, activeUserRepository)
    {
    }

    public async Task SendMessageToAllActiveUsersAsync(string message)
    {
        MessageHubModel model = MessageHubModel.ToAllUsers(message);
        await Clients.All.SendAsync(model.SignalRMessageType.GetDescription(), model);
    }
    
    public async Task SendMessageUserAsync(string connectionId, string message)
    {
        MessageHubModel model = MessageHubModel.ToUser(message);
        await Clients.Client(connectionId).SendAsync(model.SignalRMessageType.GetDescription(), model);
    }



}