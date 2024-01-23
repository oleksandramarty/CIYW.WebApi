using AutoMapper;
using CIYW.Domain.Models.Notes;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Mediator.Validators.Notes;
using CIYW.Models.Responses.Notes;
using MediatR;

namespace CIYW.Mediator.Mediator.Notes.Handlers;

public class CreateNoteCommandHandler: IRequestHandler<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Note>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Note> noteRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IEntityValidator entityValidator;

    public CreateNoteCommandHandler(
        IMapper mapper,
        IGenericRepository<Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator)
    {
        
        this.mapper = mapper;
        this.noteRepository = noteRepository;
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    public async Task<MappedHelperResponse<NoteResponse, Note>> Handle(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        this.entityValidator.ValidateRequest<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Note>>(command, () => new CreateOrUpdateNoteCommandValidator(true)); 
        Note note = this.mapper.Map<CreateOrUpdateNoteCommand, Note>(command, opts => opts.Items["IsUpdate"] = false);
        note.UserId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        await this.noteRepository.AddAsync(note, cancellationToken);

        NoteResponse result = this.mapper.Map<Note, NoteResponse>(note);
        return new MappedHelperResponse<NoteResponse, Note>(new NoteResponse
        {
            Id = note.Id,
            Name = note.Name,
            Body = note.Body,
            UserId = note.UserId,
            InvoiceId = note.InvoiceId,
        }, note);
    }
}