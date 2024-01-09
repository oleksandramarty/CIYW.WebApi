using CIYW.Domain.Models.Invoice;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.InputTypes;
using CIYW.Mediator.Mediator.Invoice.Requests;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.MutationResolver;

public class GraphQLMutationResolver: ObjectGraphType, IGraphQLMutationResolver
{
    public void CreateInvoice()
    {
        Field<InvoiceType>("createInvoice")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<InvoiceInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var cancellationToken = context.CancellationToken;
                return await this.ModifyEntityWithResultAsync<CreateInvoiceCommand, Invoice>(context, cancellationToken);
            });
    }

    public void UpdateInvoice()
    {
        Field<InvoiceType>("updateInvoice")
            .Arguments(new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" },
                    new QueryArgument<NonNullGraphType<InvoiceInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var cancellationToken = context.CancellationToken;
                return await this.ModifyEntityWithResultAsync<UpdateInvoiceCommand, Invoice>(context, cancellationToken);
            });
    }

    public void DeleteInvoice()
    {
        Field<BooleanGraphType>("deleteInvoice")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
            ))
            .ResolveAsync(async context =>
            {
                var cancellationToken = context.CancellationToken;
                await this.ModifyEntityWithoutResultAsync<DeleteInvoiceCommand>(context, cancellationToken);
                return true;
            });
    }

    private async Task<TResult> ModifyEntityWithResultAsync<TCommand, TResult>(IResolveFieldContext<object?> context, CancellationToken cancellationToken) where TCommand: IRequest<TResult>
    {
        TCommand command = context.GetArgument<TCommand>("input");
        var mediator = context.RequestServices.GetRequiredService<IMediator>();
        TResult result = await mediator.Send(command, cancellationToken);
        return result;
    }
    
    private async Task ModifyEntityWithoutResultAsync<TCommand>(IResolveFieldContext<object?> context, CancellationToken cancellationToken) where TCommand: IRequest
    {
        TCommand command = context.GetArgument<TCommand>("input");
        var mediator = context.RequestServices.GetRequiredService<IMediator>();
        await mediator.Send(command, cancellationToken);
    }
}