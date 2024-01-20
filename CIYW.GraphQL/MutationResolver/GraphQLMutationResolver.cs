using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.InputTypes;
using CIYW.Kernel.Utils;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.MutationResolver;

public class GraphQLMutationResolver: ObjectGraphType, IGraphQLMutationResolver
{
    public void CreateEntity<TType, TInputType, TCommand, TMapped, TEntity>(string name) 
        where TCommand: IRequest<MappedHelperResponse<TMapped, TEntity>> 
        where TInputType: InputObjectGraphType
        where TType: ObjectGraphType<TMapped>
    {
        Field<TType>(name)
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<TInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var cancellationToken = context.CancellationToken;
                MappedHelperResponse<TMapped, TEntity> result = await this.ModifyEntityWithResultAsync<TCommand, MappedHelperResponse<TMapped, TEntity>>(context, cancellationToken);
                return result.MappedEntity;
            });
    }
    
    public void UpdateEntity<TType, TInputType, TCommand, TMapped, TEntity, TId>(string name) 
        where TCommand: IRequest<MappedHelperResponse<TMapped, TEntity>> 
        where TInputType: InputObjectGraphType
        where TId: ScalarGraphType
        where TType: ObjectGraphType<TMapped>
    {
        Field<TType>(name)
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<TId>> { Name = "id" },
                new QueryArgument<NonNullGraphType<TInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var cancellationToken = context.CancellationToken;
                MappedHelperResponse<TMapped, TEntity> result = await this.ModifyEntityWithResultAsync<TCommand, MappedHelperResponse<TMapped, TEntity>>(context, cancellationToken, true);
                return result.MappedEntity;
            });
    }
    
    public void DeleteEntity<TCommand, TId>(string name) 
        where TCommand: IRequest
        where TId: ScalarGraphType
    {
        Field<BooleanGraphType>(name)
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<TId>> { Name = "id" }
            ))
            .ResolveAsync(async context =>
            {
                var cancellationToken = context.CancellationToken;
                await this.ModifyEntityWithoutResultAsync<TCommand>(context, cancellationToken);
                return true;
            });
    }

    private async Task<TResult> ModifyEntityWithResultAsync<TCommand, TResult>(IResolveFieldContext<object?> context, CancellationToken cancellationToken, bool isUpdate = false) where TCommand: IRequest<TResult>
    {
        TCommand command = context.GetArgument<TCommand>("input");
        if (isUpdate)
        {
            ReflectionUtils.SetValue<TCommand, Guid>(command, "Id", context.GetArgument<Guid>("id"));
        }
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