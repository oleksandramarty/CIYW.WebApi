using AutoMapper;
using CIYW.Domain.Models.Notes;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Models.Responses.Notes;
using MediatR;

namespace CIYW.Mediator.Mediator.Notes.Handlers;

public class UpdateNoteCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Note>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Note> noteRepository;

    public UpdateNoteCommandHandler(
        IMapper mapper,
        IGenericRepository<Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.noteRepository = noteRepository;
    }

    public async Task<MappedHelperResponse<NoteResponse, Note>> Handle(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        Note note = await this.noteRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Note, Guid?>(note, command.Id);
        
        Note updatedNote = this.mapper.Map<CreateOrUpdateNoteCommand, Note>(command, note, opts => opts.Items["IsUpdate"] = true);

        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        await this.HasAccess(note, userId, cancellationToken);

        await this.noteRepository.UpdateAsync(updatedNote, cancellationToken);

        NoteResponse result = this.mapper.Map<Note, NoteResponse>(updatedNote);

        return new MappedHelperResponse<NoteResponse, Note>(new NoteResponse
        {
            Id = updatedNote.Id,
            Name = updatedNote.Name,
            Body = updatedNote.Body,
            UserId = updatedNote.UserId,
            InvoiceId = updatedNote.InvoiceId,
        }, updatedNote);
    }
}