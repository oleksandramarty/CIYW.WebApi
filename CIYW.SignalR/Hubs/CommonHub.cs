using CIYW.Const.Enums;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CIYW.SignalR;

[Authorize]
public class CommonHub: Hub
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IGenericRepository<ActiveUser> activeUserRepository;

    public CommonHub(
        ICurrentUserProvider currentUserProvider, 
        IGenericRepository<ActiveUser> activeUserRepository
        )
    {
        this.currentUserProvider = currentUserProvider;
        this.activeUserRepository = activeUserRepository;
    }
    
    public override async Task OnConnectedAsync()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var connectionId = this.Context.ConnectionId;
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        await SetGroupForUserAsync(connectionId, userId, cancellationToken);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        //Guid userId = await this.GetUserIdAsync(cancellationToken);
        //await RemoveUserConnectionAsync(userId, cancellationToken);
        await base.OnDisconnectedAsync(exception);
    }
    
    private async Task SetGroupForUserAsync(string connectionId, Guid userId, CancellationToken cancellationToken)
    {
        var groups = new List<string>();

        Dictionary<SignalRGroupTypeEnum, string> roles = new Dictionary<SignalRGroupTypeEnum, string>();
        roles.Add(SignalRGroupTypeEnum.USER_GROUP, RoleProvider.User);
        roles.Add(SignalRGroupTypeEnum.ADMIN_GROUP, RoleProvider.Admin);

        foreach (var role in roles)
        {
            if (await this.currentUserProvider.HasUserInRoleAsync(role.Value, cancellationToken))
            {
                await Groups.AddToGroupAsync(connectionId, role.Key.GetDescription(), cancellationToken);
                groups.Add(role.Key.GetDescription());
            }
        }

        await AddUserConnectionAsync(userId, connectionId,groups, cancellationToken);
    }

    private async Task AddUserConnectionAsync(
        Guid userId,
        string connectionId,
        List<string> groups,
        CancellationToken cancellationToken
    )
    {
        await this.activeUserRepository.AddAsync(new ActiveUser
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            UserId = userId,
            ConnectionId = connectionId,
            Groups = string.Join(',', groups)
        }, cancellationToken);
    }
    
    private async Task RemoveUserConnectionAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        ActiveUser activeUser = await this.activeUserRepository.GetByPropertyAsync(a => a.UserId == userId, cancellationToken);

        var groupsOld = activeUser.Groups.NotNullOrEmpty()
            ? activeUser.Groups.Split(",").ToList()
            : new List<string>();
        
        if (groupsOld.Any())
        {
            foreach (var group in groupsOld)
            {
                await Groups.RemoveFromGroupAsync(activeUser.ConnectionId, group, cancellationToken);
            }
        }

        await this.activeUserRepository.DeleteAsync(activeUser.Id, cancellationToken);

    }

    private async Task<Guid> GetUserIdAsync(CancellationToken cancellationToken)
    {
        var userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        return userId;
    }
}