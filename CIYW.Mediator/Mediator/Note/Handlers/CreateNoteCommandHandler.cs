﻿using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Mediator.Validators.Notes;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Handlers;

public class CreateNoteCommandHandler: IRequestHandler<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>
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

    public async Task<Domain.Models.Note.Note> Handle(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        this.entityValidator.ValidateRequest<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command, () => new CreateOrUpdateNoteCommandValidator(true)); 
        Domain.Models.Note.Note note = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command, opts => opts.Items["IsUpdate"] = false);
        note.UserId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        await this.noteRepository.AddAsync(note, cancellationToken);

        return note;
    }
}