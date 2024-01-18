using AutoMapper;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.ListWithIncludeHelper;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using CIYW.Models.Responses.Users;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.QueryResolver;

public class GraphQLQueryResolver: ObjectGraphType, IGraphQLQueryResolver
{
    // TODO refactor for GENERIC
    public void GetCurrencyById()
    {
        Field<CurrencyType>("currency")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Currency.Currency>>();
                
                var cancellationToken = context.CancellationToken;

                Currency result = await repository.GetByIdAsync(entityId, cancellationToken);
                return result;
            });
    }

    public void GetInvoiceById()
    {
        Field<InvoiceType>("invoice")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var cancellationToken = context.CancellationToken;
                MappedHelperResponse<InvoiceResponse, Invoice> mapped = 
                    await mediator.Send(
                        new GetInvoiceByIdQuery(context.GetArgument<Guid>("id", default)), 
                        cancellationToken);
                return mapped.MappedEntity;
            });
    }
    public void GetCategoryById()
    {
        Field<CategoryType>("category")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Category.Category>>();
                
                var cancellationToken = context.CancellationToken;

                Category result = await repository.GetByIdAsync(entityId, cancellationToken);
                return result;
            });
    }
    
    public void GetUserById()
    {
        Field<UserType>("user")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.User.User>>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                User result = await repository.GetWithIncludeAsync(u => u.Id == entityId, cancellationToken,
                    query => query.Include(u => u.Tariff),
                    query => query.Include(u => u.Currency),
                    query => query.Include(u => u.Invoices),
                    query => query.Include(u => u.UserBalance));
                return result;
            });
    }
    
    public void GetUserByIdForAdmin()
    {
        Field<UserType>("userByIdForAdmin")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                MappedHelperResponse<UserResponse, User> result = await mediator.Send(new UserByIdQuery(entityId), cancellationToken);
                return result.MappedEntity;
            });
    }
    
    public void GetUserBalanceByUserId()
    {
        Field<UserBalanceType>("userBalance")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "userId" }))
            .ResolveAsync(async context =>
            {
                Guid userId = context.GetArgument<Guid>("userId", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.User.UserBalance>>();
                var mapper = context.RequestServices.GetRequiredService<IMapper>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                UserBalance result = await repository.GetWithIncludeAsync(u => u.UserId == userId, cancellationToken,
                    query => query.Include(u => u.User),
                    query => query.Include(u => u.Currency));
                return mapper.Map<UserBalance, UserBalanceResponse>(result);
            });
    }
    public void GetInvoiceHistory()
    {
        Field<ListInvoicesWithIncludeHelperType>("invoices")
            .Arguments(new QueryArguments(GetPageableQueryArguments()))
            .ResolveAsync(async context => await this.GetPageableResponse<InvoiceResponse, UserInvoicesQuery>(context, new UserInvoicesQuery(this.GetBaseFilterQuery(context))));
    }
    
    public void GetUsers()
    {
        Field<ListUsersWithIncludeHelperType>("users")
            .Arguments(new QueryArguments(GetPageableQueryArguments()))
            .ResolveAsync(async context => await this.GetPageableResponse<UserResponse, UsersQuery>(context, new UsersQuery(this.GetBaseFilterQuery(context))));
    }

    private async Task<ListWithIncludeHelper<T>> GetPageableResponse<T, TQuery>(IResolveFieldContext<object?> context, TQuery query)
        where TQuery : BaseFilterQuery, IRequest<ListWithIncludeHelper<T>>
    {
        var cancellationToken = context.CancellationToken;
        var mediator = context.RequestServices.GetRequiredService<IMediator>();
        var result = await mediator.Send(query, cancellationToken);
        return result;
    }
    
    private IEnumerable<QueryArgument> GetPageableQueryArguments()
    {
        return new QueryArguments(new QueryArgument<BooleanGraphType> { Name = "isFull" },
            new QueryArgument<IntGraphType> { Name = "pageNumber" },
            new QueryArgument<IntGraphType> { Name = "pageSize" },
            new QueryArgument<DateTimeGraphType> { Name = "dateFrom" },
            new QueryArgument<DateTimeGraphType> { Name = "dateTo" },
            new QueryArgument<StringGraphType> { Name = "parentClass" },
            new QueryArgument<StringGraphType> { Name = "column" },
            new QueryArgument<StringGraphType> { Name = "direction" });
    }

    private BaseFilterQuery GetBaseFilterQuery(IResolveFieldContext<object?> context)
    {
        BaseFilterQuery query = new BaseFilterQuery();
        query.Ids = null;
        query.Paginator = new Paginator()
        {
            IsFull = context.GetArgument<bool?>("isFull") ?? false,
            PageNumber = context.GetArgument<int?>("pageNumber") ?? 1,
            PageSize = context.GetArgument<int?>("pageSize") ?? 5,
        };
        query.Sort = new BaseSortableQuery
        {
            ParentClass = context.GetArgument<string?>("parentClass") ?? null,
            Column = context.GetArgument<string?>("column") ?? "Date",
            Direction = context.GetArgument<string?>("direction") ?? "desc"
        };
                
        var dateFrom = context.GetArgument<DateTime?>("dateFrom");
        var dateTo = context.GetArgument<DateTime?>("dateTo");

        if (dateFrom.HasValue || dateTo.HasValue)
        {
            query.DateRange = new BaseDateRangeQuery
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }
        else
        {
            query.DateRange = null;
        }

        return query;
    }
}