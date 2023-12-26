using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Note;

public class DeleteNoteCommandHandler: IRequestHandler<DeleteNoteCommand>
{
    private readonly IGenericRepository<Domain.Models.Note.Note> noteRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IEntityValidator entityValidator;

    public DeleteNoteCommandHandler(
        IGenericRepository<Domain.Models.Note.Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator)
    {
        this.noteRepository = noteRepository;
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    public async Task Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = await this.noteRepository.GetByIdAsync(command.Id, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Note.Note, Guid?>(note, command.Id);

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        EntityExtension.HasAccess(note, userId);

        await this.noteRepository.DeleteAsync(command.Id, cancellationToken);
    }
}