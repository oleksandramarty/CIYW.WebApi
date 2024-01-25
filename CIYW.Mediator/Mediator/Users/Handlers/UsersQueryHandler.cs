using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Interfaces.Strategies;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UsersQueryHandler : BasePageableHelper<User>, IRequestHandler<UsersQuery, ListWithIncludeHelper<UserResponse>>
{
    private readonly IPageableStrategyRepository<UserResponse, UsersQuery> userStrategy;
    public UsersQueryHandler(
        IReadGenericRepository<User> userRepository, 
        IPageableStrategyRepository<UserResponse, UsersQuery> userStrategy): base(userRepository)
    {
        this.userStrategy = userStrategy;
    }
    
    public async Task<ListWithIncludeHelper<UserResponse>> Handle(UsersQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<UserResponse> result = await this.userStrategy.GetPageableAsync(query, cancellationToken);

        return result;

        // return await this.GetPageableResponseAsync<UserResponse>(query.DateRange != null
        //         ? q => !query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue &&
        //                q.Created >= query.DateRange.DateFrom.Value &&
        //                !query.DateRange.DateTo.HasValue ||
        //                query.DateRange.DateTo.HasValue && q.Created <= query.DateRange.DateTo.Value
        //         : null,
        //     query,
        //     cancellationToken,
        //     q => q.Include(u => u.UserBalance).ThenInclude(u => u.Currency));
    }
}
