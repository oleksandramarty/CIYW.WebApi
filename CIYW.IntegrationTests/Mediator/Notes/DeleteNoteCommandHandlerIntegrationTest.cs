using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Notes.Handlers;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Notes;

[TestFixture]
public class DeleteNoteCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidCreateNoteCommand_ReturnsNoteId()
    {
        // Arrange
        var command = MockCommandQueryHelper.CreateNoteCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Notes.Note note = dbContext.Notes.FirstOrDefault(i => i.UserId == InitConst.MockUserId);
            
            var handler = new DeleteNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Notes.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await handler.Handle(new DeleteNoteCommand(note.Id), CancellationToken.None);

            // Assert
            dbContext.Notes.Count(u => u.Id == note.Id).Should().Be(0);
        }
    }
    
    [Test]
    public async Task Handle_InvalidDeleteNoteCommand_ReturnsExceptionNotFound()
    {
        // Arrange
        Guid noteId = Guid.NewGuid();
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new DeleteNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Notes.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<DeleteNoteCommand, LoggerException>(
                handler, 
                new DeleteNoteCommand(noteId), 
                String.Format(ErrorMessages.EntityWithIdNotFound, nameof(Domain.Models.Notes.Note), noteId));
        }
    }
    
    [Test]
    public async Task Handle_InvalidDeleteNoteCommand_ReturnsExceptionForbidden()
    {
        // Arrange
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Notes.Note note = dbContext.Notes.FirstOrDefault(i => i.UserId == InitConst.MockAuthUserId);
            
            var handler = new DeleteNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Notes.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<DeleteNoteCommand, LoggerException>(
                handler, 
                new DeleteNoteCommand(note.Id), 
                ErrorMessages.Forbidden);
        }
    }
}