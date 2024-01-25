using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Auth.Requests;
using FluentValidation;

namespace CIYW.Mediator.Validators.Auth;

public class ForgotPasswordQueryValidator: AbstractValidator<ForgotPasswordQuery>
{
    public ForgotPasswordQueryValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(ForgotPasswordQuery.Login)));
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(ForgotPasswordQuery.Email)));
        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(ForgotPasswordQuery.Phone)));
    }
}