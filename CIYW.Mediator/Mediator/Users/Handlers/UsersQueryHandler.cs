using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UsersQueryHandler(IReadGenericRepository<Domain.Models.User.User> userRepository)
    : BasePageableHelper<User>(userRepository),
        IRequestHandler<UsersQuery, ListWithIncludeHelper<Domain.Models.User.User>>
{
    public async Task<ListWithIncludeHelper<Domain.Models.User.User>> Handle(UsersQuery query, CancellationToken cancellationToken)
    {
        return await this.GetPageableResponseAsync(query.DateRange != null
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
