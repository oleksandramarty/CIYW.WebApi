using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;
using CIYW.GraphQL.Types;
using CIYW.Interfaces;
using CIYW.Models.Requests.Common;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.QueryResolver;

public class GraphQLQueryResolver: ObjectGraphType, IGraphQLQueryResolver
{
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
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Invoice.Invoice>>();

                var cancellationToken = context.CancellationToken;

                Invoice result = await repository.GetWithIncludeAsync(u => u.Id == entityId, cancellationToken,
                    query => query.Include(u => u.Note),
                    query => query.Include(u => u.Currency),
                    query => query.Include(u => u.Category));
                return result;
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
    public void GetUserBalanceByUserId()
    {
        Field<UserBalanceType>("userBalance")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "userId" }))
            .ResolveAsync(async context =>
            {
                Guid userId = context.GetArgument<Guid>("userId", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.User.UserBalance>>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                UserBalance result = await repository.GetWithIncludeAsync(u => u.UserId == userId, cancellationToken,
                    query => query.Include(u => u.User),
                    query => query.Include(u => u.Currency));
                return result;
            });
    }
    public void GetInvoiceHistory()
    {
        Field<ListWithIncludeHelperType<Invoice, InvoiceType>>("invoices")
            .Arguments(new QueryArguments(new QueryArguments(new QueryArgument<BooleanGraphType> { Name = "isFull" },
                new QueryArgument<IntGraphType> { Name = "pageNumber" },
                new QueryArgument<IntGraphType> { Name = "pageSize" },
                new QueryArgument<DateTimeGraphType> { Name = "dateFrom" },
                new QueryArgument<DateTimeGraphType> { Name = "dateTo" },
                new QueryArgument<StringGraphType> { Name = "column" },
                new QueryArgument<StringGraphType> { Name = "direction" })))
            .ResolveAsync(async context =>
            {
                BaseFilterQuery query = this.GetBaseFilterQuery(context);
                
                var cancellationToken = context.CancellationToken;

                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Invoice.Invoice>>();
                
                var result = await repository.GetListWithIncludeAsync(
                    query.DateRange != null 
                        ? q => !query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue && q.Date >= query.DateRange.DateFrom.Value &&
                            !query.DateRange.DateTo.HasValue || query.DateRange.DateTo.HasValue && q.Date <= query.DateRange.DateTo.Value
                        : null,
                    query,
                    cancellationToken,
                    q => q.Include(u => u.Category),
                    q => q.Include(u => u.Currency),
                    q => q.Include(u => u.Note));
                return result;
            });
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