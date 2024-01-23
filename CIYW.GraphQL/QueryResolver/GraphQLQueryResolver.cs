using AutoMapper;
using CIYW.Domain.Models.Categories;
using CIYW.Domain.Models.Currencies;
using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Users;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.ListWithIncludeHelper;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoices;
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
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Currency>>();
                
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
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Category>>();
                
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
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                MappedHelperResponse<UserResponse, User> result = await mediator.Send(new UserByIdQuery(entityId), cancellationToken);
                return result.MappedEntity;
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
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<UserBalance>>();
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
            .Arguments(new QueryArguments(GetPageableUsersQueryArguments()))
            .ResolveAsync(async context =>
            {
                var query = new UsersQuery(this.GetBaseFilterQuery(context));

                query.IsBlocked = context.GetArgument<bool?>("isBlocked") ?? false;
                query.Phone = context.GetArgument<string?>("phone") ?? null;
                query.Email = context.GetArgument<string?>("email") ?? null;
                query.Login = context.GetArgument<string?>("login") ?? null;
                query.Name = context.GetArgument<string?>("name") ?? null;

                query.CreatedRange = this.GetDateRangeModel(context, "createdRange");
                query.UpdatedRange = this.GetDateRangeModel(context, "updatedRange");

                query.CurrencyIds = this.GetIdsModel(context, "currencyIds");
                query.RoleIds = this.GetIdsModel(context, "roleIds");
                query.TariffIds = this.GetIdsModel(context, "tariffIds");
                
                return await this.GetPageableResponse<UserResponse, UsersQuery>(context, query);
            });
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

    private IEnumerable<QueryArgument> GetPageableUsersQueryArguments()
    {
        return this.GetPageableQueryArguments().Concat(
            new QueryArguments(new QueryArgument<BooleanGraphType> { Name = "isBlocked" },
                new QueryArgument<StringGraphType> { Name = "phone" },
                new QueryArgument<StringGraphType> { Name = "email" },
                new QueryArgument<StringGraphType> { Name = "login" },
                new QueryArgument<StringGraphType> { Name = "name" },
                new QueryArgument<DateTimeGraphType> { Name = "createdRangeFrom" },
                new QueryArgument<DateTimeGraphType> { Name = "createdRangeTo" },
                new QueryArgument<DateTimeGraphType> { Name = "updatedRangeFrom" },
                new QueryArgument<DateTimeGraphType> { Name = "updatedRangeTo" },
                new QueryArgument<ListGraphType<GuidGraphType>> { Name = "currencyIds" },
                new QueryArgument<ListGraphType<GuidGraphType>> { Name = "roleIds" },
                new QueryArgument<ListGraphType<GuidGraphType>> { Name = "tariffIds" }));
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

    private BaseDateRangeQuery GetDateRangeModel(IResolveFieldContext<object?> context, string name)
    {
        return new BaseDateRangeQuery
        {
            DateFrom = context.GetArgument<DateTime?>($"{name}From"),
            DateTo = context.GetArgument<DateTime?>($"{name}To")
        };
    }
    private BaseIdsListQuery GetIdsModel(IResolveFieldContext<object?> context, string name)
    {
        List<Guid?> ids = context.GetArgument<List<Guid?>>(name);
        return ids != null && ids.Any(x => x.HasValue) ? new BaseIdsListQuery
        {
            Ids = ids.Where(x => x.HasValue).Select(x => x.Value).ToList()
        } : null;
    }
}