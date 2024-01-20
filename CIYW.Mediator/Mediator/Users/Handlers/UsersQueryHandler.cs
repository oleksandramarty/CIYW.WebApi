using CIYW.Domain.Models.User;
using CIYW.Elasticsearch.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UsersQueryHandler : BasePageableHelper<User>, IRequestHandler<UsersQuery, ListWithIncludeHelper<UserResponse>>
{
    private readonly IElasticSearchRepository elastic;
    public UsersQueryHandler(
        IReadGenericRepository<User> userRepository, 
        IElasticSearchRepository elastic): base(userRepository)
    {
        this.elastic = elastic;
    }
    
    public async Task<ListWithIncludeHelper<UserResponse>> Handle(UsersQuery query, CancellationToken cancellationToken)
    {
        return await this.elastic.GetPageableResponseAsync<UserSearchModel, UserResponse>(
            null, 
            null, 
            this.CreateSortDescriptor(query.Sort),
            query, 
            cancellationToken);
        
        
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

    private SortDescriptor<UserSearchModel> CreateSortDescriptor(BaseSortableQuery? filter)
    {
        if (filter == null || filter.Column.NullOrEmpty())
        {
            return null;
        }

        SortDescriptor<UserSearchModel> sort = new SortDescriptor<UserSearchModel>();

        switch (filter.Column)
        {
            case nameof(UserSearchModel.Created):
                return filter.Direction == "asc" ? sort.Ascending(t => t.Created) : sort.Descending(t => t.Created);
            case nameof(UserSearchModel.UserBalance.Amount):
                return filter.Direction == "asc" ? sort.Ascending(t => t.UserBalance.Amount) : sort.Descending(t => t.UserBalance.Amount);
            default:
                return null;
        }
    }
}
