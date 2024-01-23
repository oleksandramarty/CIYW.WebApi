using CIYW.Const.Errors;
using CIYW.Mediator.Mediator.Notes.Request;
using FluentValidation;

namespace CIYW.Mediator.Validators.Notes;

public class CreateOrUpdateNoteCommandValidator: AbstractValidator<CreateOrUpdateNoteCommand>
{
    public CreateOrUpdateNoteCommandValidator(bool isNew)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Name)))
            .MaximumLength(50).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateNoteCommand.Name),
                50));

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Body)))
            .MaximumLength(500).WithMessage(String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateNoteCommand.Body),
                500));
        
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Id)))
            .When(x => !isNew);
    }
}