using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Category.Requests;
using FluentValidation;

namespace CIYW.Mediator.Validators.Categories;

public class CreateOrUpdateCategoryCommandValidator: AbstractValidator<CreateOrUpdateCategoryCommand>
{
    public CreateOrUpdateCategoryCommandValidator(bool isNew)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCategoryCommand.Name)))
            .MaximumLength(50).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCategoryCommand.Name),
                50));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCategoryCommand.Description)))
            .MaximumLength(500).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCategoryCommand.Description),
                500));
        
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCategoryCommand.Id)))
            .When(x => !isNew);
    }
}