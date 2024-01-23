using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Notes.Handlers;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Models.Responses.Notes;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Notes;

[TestFixture]
public class UpdateNoteCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidCreateNoteCommand_ReturnsNoteId()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Notes.Note note = dbContext.Notes.FirstOrDefault(i => i.UserId == InitConst.MockUserId);
            command.Id = note.Id;
            command.Name = "New Name";
            
            var handler = new UpdateNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Notes.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            MappedHelperResponse<NoteResponse, Domain.Models.Notes.Note> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Notes.Count(u => u.Id == result.Entity.Id && u.Name == command.Name).Should().Be(1);
        }
    }
    
    [Test]
    public async Task Handle_InvalidUpdateNoteCommand_ReturnsExceptionNotFound()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Id = Guid.NewGuid();
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UpdateNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Notes.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Domain.Models.Notes.Note>, LoggerException>(
                handler, 
                command, 
                String.Format(ErrorMessages.EntityWithIdNotFound, nameof(Domain.Models.Notes.Note), command.Id));
        }
    }
    
    [Test]
    public async Task Handle_InvalidUpdateNoteCommand_ReturnsExceptionForbidden()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Notes.Note note = dbContext.Notes.FirstOrDefault(i => i.UserId == InitConst.MockAuthUserId);

            command.Id = note.Id;
            
            var handler = new UpdateNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Notes.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateNoteCommand, MappedHelperResponse<NoteResponse, Domain.Models.Notes.Note>, LoggerException>(
                handler, 
                command, 
                ErrorMessages.Forbidden);
        }
    }
}