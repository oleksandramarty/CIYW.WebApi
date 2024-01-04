using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Currency.Requests;
using FluentValidation;

namespace CIYW.Mediator.Validators.Currencies;

public class CreateOrUpdateCurrencyCommandValidator: AbstractValidator<CreateOrUpdateCurrencyCommand>
{
    public CreateOrUpdateCurrencyCommandValidator(bool isNew)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.Name)))
            .MaximumLength(50).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCurrencyCommand.Name),
                50));

        RuleFor(x => x.Symbol)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.Symbol)))
            .MaximumLength(3).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCurrencyCommand.Symbol),
                3));
        
        RuleFor(x => x.IsoCode)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.IsoCode)))
            .Length(3).WithMessage(String.Format(ErrorMessages.FieldExactLengthError, nameof(CreateOrUpdateCurrencyCommand.IsoCode),
                3));
        
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.Id)))
            .When(x => !isNew);
    }
}