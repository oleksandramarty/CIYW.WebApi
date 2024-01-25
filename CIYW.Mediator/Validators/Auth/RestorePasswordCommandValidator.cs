using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Auth.Requests;
using FluentValidation;

namespace CIYW.Mediator.Validators.Auth;

public class RestorePasswordCommandValidator: AbstractValidator<RestorePasswordCommand>
{
    public RestorePasswordCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Login)));
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Email)));
        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Phone)));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Password)));
        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.ConfirmPassword)))
            .Equal(x => x.Password)
            .WithMessage(ErrorMessages.PasswordsDoesntMatch);
        
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Url)))
            .MinimumLength(114).WithMessage(String.Format(ErrorMessages.FieldExactLengthError, nameof(RestorePasswordCommand.Url),
                114))
            .MaximumLength(114).WithMessage(String.Format(ErrorMessages.FieldExactLengthError, nameof(RestorePasswordCommand.Url),
                114));
    }
}