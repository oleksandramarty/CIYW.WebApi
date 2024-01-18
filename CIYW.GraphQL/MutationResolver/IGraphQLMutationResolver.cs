using CIYW.Mediator;
using GraphQL.Types;
using MediatR;

namespace CIYW.GraphQL.MutationResolver;

public interface IGraphQLMutationResolver
{
    void CreateEntity<TType, TInputType, TCommand, TMapped, TEntity>(string name) 
        where TCommand: IRequest<MappedHelperResponse<TMapped, TEntity>> 
        where TInputType: InputObjectGraphType
        where TType: ObjectGraphType<TMapped>;

    void UpdateEntity<TType, TInputType, TCommand, TMapped, TEntity, TId>(string name) 
        where TCommand: IRequest<MappedHelperResponse<TMapped, TEntity>> 
        where TInputType: InputObjectGraphType
        where TId: ScalarGraphType
        where TType: ObjectGraphType<TMapped>;

    void DeleteEntity<TCommand, TId>(string name)
        where TCommand : IRequest
        where TId : ScalarGraphType;
}