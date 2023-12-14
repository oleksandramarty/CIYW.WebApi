using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediatr.Note.Request;
using MediatR;

namespace CIYW.Mediatr.Note;

public class CreateNoteCommandHandler: IRequestHandler<CreateNoteCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Domain.Models.Note> _noteRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateNoteCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Note> noteRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        _noteRepository = noteRepository;
        _currentUserProvider = currentUserProvider;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note note = this._mapper.Map<CreateNoteCommand, Domain.Models.Note>(command);
        note.UserId = await this._currentUserProvider.GetUserIdAsync(cancellationToken);
        await this._noteRepository.AddAsync(note, cancellationToken);

        return note.Id;
    }
}