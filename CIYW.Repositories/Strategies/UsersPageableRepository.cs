using CIYW.Interfaces.Strategies;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Users;

namespace CIYW.Repositories.Strategies;

public class UsersPageableRepository: IPageableStrategyRepository<UserResponse, UsersQuery>
{
    private readonly IPageableStrategy<UserResponse, UsersQuery> strategy;

    public UsersPageableRepository(IPageableStrategy<UserResponse, UsersQuery> strategy)
    {
        this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public async Task<ListWithIncludeHelper<UserResponse>> GetPageableAsync(UsersQuery filter, CancellationToken cancellationToken)
    {
        return await strategy.GetPageableAsync(filter, cancellationToken);
    }
}