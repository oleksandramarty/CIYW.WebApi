using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediatr.Note.Request;
using MediatR;

namespace CIYW.Mediatr.Note;

public class DeleteNoteCommandHandler: IRequestHandler<DeleteNoteCommand>
{
    private readonly IGenericRepository<Domain.Models.Note.Note> noteRepository;
    private readonly ICurrentUserProvider currentUserProvider;

    public DeleteNoteCommandHandler(
        IGenericRepository<Domain.Models.Note.Note> noteRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        this.noteRepository = noteRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = await this.noteRepository.GetByIdAsync(command.Id, cancellationToken);

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        EntityExtension.HasAccess(note, userId);

        await this.noteRepository.DeleteAsync(command.Id, cancellationToken);
    }
}