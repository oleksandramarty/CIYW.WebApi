using CIYW.Kernel.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CIYW.TestHelper;

public static class TestUtilities
{
    public static async Task Handle_InvalidCommand<TCommand, TResult, TException>(
        IRequestHandler<TCommand, TResult> handler, TCommand command, string errorMessage)
        where TCommand : IRequest<TResult>
        where TException : Exception
    {
        var exception = await Assert.ThrowsExceptionAsync<TException>(
            () => handler.Handle(command, CancellationToken.None)
        );
        StringAssert.Contains(exception.Message, errorMessage);
    }
    
    public static async Task Handle_InvalidCommand<TCommand, TException>(
        IRequestHandler<TCommand> handler, TCommand command, string errorMessage)
        where TCommand : IRequest
        where TException : Exception
    {
        var exception = await Assert.ThrowsExceptionAsync<TException>(
            () => handler.Handle(command, CancellationToken.None)
        );
        StringAssert.Contains(exception.Message, errorMessage);
    }

    public static void Validate_Command<TCommand, TResult>(
        TCommand command, Func<IValidator<TCommand>> validatorFactory, string[]? expectedErrors)
        where TCommand : IRequest<TResult>
    {
        IValidator<TCommand> validator = validatorFactory.Invoke();
        ValidationResult validationResult = validator.Validate(command);
        
        if (expectedErrors == null)
        {
            Assert.IsTrue(validationResult.IsValid);
        }
        else
        {
            Assert.IsFalse(validationResult.IsValid);
            Assert.IsTrue(validationResult.Errors.All(item => expectedErrors.Contains(item.ErrorMessage)));
        }
    }
}
