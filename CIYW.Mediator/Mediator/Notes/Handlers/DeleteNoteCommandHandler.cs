using AutoMapper;
using CIYW.Domain.Models.Notes;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Notes.Request;
using MediatR;

namespace CIYW.Mediator.Mediator.Notes.Handlers;

public class DeleteNoteCommandHandler: UserEntityValidatorHelper, IRequestHandler<DeleteNoteCommand>
{
    private readonly IGenericRepository<Note> noteRepository;

    public DeleteNoteCommandHandler(
        IMapper mapper,
        IGenericRepository<Note> noteRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.noteRepository = noteRepository;
    }

    public async Task Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
    {
        Note note = await this.noteRepository.GetByIdAsync(command.Id, cancellationToken);

        this.ValidateExist<Note, Guid?>(note, command.Id);

        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        await this.HasAccess(note, userId, cancellationToken);

        await this.noteRepository.DeleteAsync(command.Id, cancellationToken);
    }
}