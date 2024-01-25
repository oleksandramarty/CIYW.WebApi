using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Auth.Requests;
using FluentValidation;

namespace CIYW.Mediator.Validators.Auth;

public class ChangePasswordCommandValidator: AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(ChangePasswordCommand.OldPassword)));
        
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(ChangePasswordCommand.NewPassword)));
        
        RuleFor(x => x.ConfirmationPassword)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(ChangePasswordCommand.ConfirmationPassword)))
            .Equal(x => x.NewPassword)
            .WithMessage(ErrorMessages.PasswordsDoesntMatch);
    }
}