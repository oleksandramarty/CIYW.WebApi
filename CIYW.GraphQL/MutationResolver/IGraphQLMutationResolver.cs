using GraphQL.Types;
using MediatR;

namespace CIYW.GraphQL.MutationResolver;

public interface IGraphQLMutationResolver
{
    void CreateEntity<TType, TInputType, TCommand, TResult>(string name)
        where TCommand : IRequest<TResult>
        where TInputType : InputObjectGraphType
        where TType : ObjectGraphType<TResult>;

    void UpdateEntity<TType, TInputType, TCommand, TResult, TId>(string name)
        where TCommand : IRequest<TResult>
        where TInputType : InputObjectGraphType
        where TId : ScalarGraphType
        where TType : ObjectGraphType<TResult>;

    void DeleteEntity<TCommand, TId>(string name)
        where TCommand : IRequest
        where TId : ScalarGraphType;
}