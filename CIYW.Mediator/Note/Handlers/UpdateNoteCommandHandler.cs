using AutoMapper;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Note;

public class UpdateNoteCommandHandler: IRequestHandler<UpdateNoteCommand>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Note.Note> noteRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IEntityValidator entityValidator;

    public UpdateNoteCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Note.Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.noteRepository = noteRepository;
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    public async Task Handle(UpdateNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = await this.noteRepository.GetByIdAsync(command.Id, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Note.Note, Guid?>(note, command.Id);
        
        Domain.Models.Note.Note updatedNote = this.mapper.Map<UpdateNoteCommand, Domain.Models.Note.Note>(command, note);

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        EntityExtension.HasAccess(note, userId);

        await this.noteRepository.UpdateAsync(updatedNote, cancellationToken);
    }
}