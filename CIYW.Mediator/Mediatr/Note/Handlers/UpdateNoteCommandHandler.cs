using AutoMapper;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediatr.Note.Request;
using MediatR;

namespace CIYW.Mediator.Mediatr.Note.Handlers;

public class UpdateNoteCommandHandler: IRequestHandler<CreateOrUpdateNoteCommand, Guid>
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

    public async Task<Guid> Handle(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = await this.noteRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Note.Note, Guid?>(note, command.Id);
        
        Domain.Models.Note.Note updatedNote = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command, note, opts => opts.Items["IsUpdate"] = true);

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        EntityExtension.HasAccess(note, userId);

        await this.noteRepository.UpdateAsync(updatedNote, cancellationToken);

        return note.Id;
    }
}