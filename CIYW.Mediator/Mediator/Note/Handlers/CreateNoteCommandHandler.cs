using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Mediator.Validators.Notes;
using CIYW.Models.Responses.Note;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Handlers;

public class CreateNoteCommandHandler: IRequestHandler<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Note.Note> noteRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IEntityValidator entityValidator;

    public CreateNoteCommandHandler(
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

    public async Task<MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>> Handle(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        this.entityValidator.ValidateRequest<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>>(command, () => new CreateOrUpdateNoteCommandValidator(true)); 
        Domain.Models.Note.Note note = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command, opts => opts.Items["IsUpdate"] = false);
        note.UserId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        await this.noteRepository.AddAsync(note, cancellationToken);

        NoteResponse result = this.mapper.Map<Domain.Models.Note.Note, NoteResponse>(note);
        return new MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>(new NoteResponse
        {
            Id = note.Id,
            Name = note.Name,
            Body = note.Body,
            UserId = note.UserId,
            InvoiceId = note.InvoiceId,
        }, note);
    }
}