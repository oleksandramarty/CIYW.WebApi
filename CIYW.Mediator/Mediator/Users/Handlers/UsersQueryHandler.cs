using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UsersQueryHandler : BasePageableHelper<User>, IRequestHandler<UsersQuery, ListWithIncludeHelper<UserResponse>>
{
    public UsersQueryHandler(IReadGenericRepository<User> userRepository): base(userRepository)
    {
    }
    
    public async Task<ListWithIncludeHelper<UserResponse>> Handle(UsersQuery query, CancellationToken cancellationToken)
    {
        return await this.GetPageableResponseAsync<UserResponse>(query.DateRange != null
                ? q => !query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue &&
                       q.Created >= query.DateRange.DateFrom.Value &&
                       !query.DateRange.DateTo.HasValue ||
                       query.DateRange.DateTo.HasValue && q.Created <= query.DateRange.DateTo.Value
                : null,
            query,
            cancellationToken,
            q => q.Include(u => u.UserBalance).ThenInclude(u => u.Currency));
    }
}
