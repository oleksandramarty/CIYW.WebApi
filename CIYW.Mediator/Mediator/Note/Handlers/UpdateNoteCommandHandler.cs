using AutoMapper;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Models.Responses.Note;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Handlers;

public class UpdateNoteCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Note.Note> noteRepository;

    public UpdateNoteCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Note.Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.noteRepository = noteRepository;
    }

    public async Task<MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>> Handle(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = await this.noteRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Domain.Models.Note.Note, Guid?>(note, command.Id);
        
        Domain.Models.Note.Note updatedNote = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command, note, opts => opts.Items["IsUpdate"] = true);

        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        this.HasAccess(note, userId);

        await this.noteRepository.UpdateAsync(updatedNote, cancellationToken);

        NoteResponse result = this.mapper.Map<Domain.Models.Note.Note, NoteResponse>(updatedNote);

        return new MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>(new NoteResponse
        {
            Id = updatedNote.Id,
            Name = updatedNote.Name,
            Body = updatedNote.Body,
            UserId = updatedNote.UserId,
            InvoiceId = updatedNote.InvoiceId,
        }, updatedNote);
    }
}