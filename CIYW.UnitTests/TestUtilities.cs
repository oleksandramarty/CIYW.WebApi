using CIYW.Kernel.Exceptions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CIYW.UnitTests;

public static class TestUtilities
{
    public static async Task Handle_InvalidCommand<TCommand, TResult>(
        IRequestHandler<TCommand, TResult> handler, TCommand command, string errorMessage)
        where TCommand : IRequest<TResult>
    {
        var exception = await Assert.ThrowsExceptionAsync<LoggerException>(
            () => handler.Handle(command, CancellationToken.None)
        );
        StringAssert.Contains(exception.Message, errorMessage);
    }
}