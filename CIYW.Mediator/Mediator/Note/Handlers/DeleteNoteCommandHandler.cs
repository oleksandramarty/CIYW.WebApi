using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Handlers;

public class DeleteNoteCommandHandler: UserEntityValidatorHelper, IRequestHandler<DeleteNoteCommand>
{
    private readonly IGenericRepository<Domain.Models.Note.Note> noteRepository;

    public DeleteNoteCommandHandler(
        IGenericRepository<Domain.Models.Note.Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator): base(entityValidator, currentUserProvider)
    {
        this.noteRepository = noteRepository;
    }

    public async Task Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = await this.noteRepository.GetByIdAsync(command.Id, cancellationToken);

        this.ValidateExist<Domain.Models.Note.Note, Guid?>(note, command.Id);

        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        this.HasAccess(note, userId);

        await this.noteRepository.DeleteAsync(command.Id, cancellationToken);
    }
}