using AutoMapper;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Mediator.Mediatr.Note.Handlers;
using CIYW.Mediator.Mediatr.Note.Request;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Note;

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
            
            var handlerAdd = new CreateNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Note.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );
            
            var handlerDelete = new DeleteNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Note.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            Guid noteId = await handlerAdd.Handle(command, CancellationToken.None);
            await handlerDelete.Handle(new DeleteNoteCommand(noteId), CancellationToken.None);

            // Assert
            dbContext.Notes.Count(u => u.Id == noteId).Should().Be(0);
        }
    }
}