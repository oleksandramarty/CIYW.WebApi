using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Categories.Requests;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using FluentValidation;

namespace CIYW.Mediator.Validators.Tariffs;

public class CreateOrUpdateTariffCommandValidator: AbstractValidator<CreateOrUpdateTariffCommand>
{
    public CreateOrUpdateTariffCommandValidator(bool isNew)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateTariffCommand.Name)))
            .MaximumLength(50).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateTariffCommand.Name),
                50));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateTariffCommand.Description)))
            .MaximumLength(500).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateTariffCommand.Description),
                500));
        
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateTariffCommand.Id)))
            .When(x => !isNew);
    }
}