using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions.Validators.Note;
using FluentValidation;
using FluentValidation.Results;
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

    public static void Validate_InvalidCommand<TCommand, TResult>(
        TCommand command, Func<IValidator<TCommand>> validatorFactory, string[] expectedErrors)
        where TCommand : IRequest<TResult>
    {
        IValidator<TCommand> validator = validatorFactory.Invoke();
        ValidationResult validationResult = validator.Validate(command);

        Assert.IsFalse(validationResult.IsValid);
        Assert.IsTrue(validationResult.Errors.All(item => expectedErrors.Contains(item.ErrorMessage)));
    }

}