using AutoMapper;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Note.Handlers;
using CIYW.Models.Responses.Note;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Note;

[TestFixture]
public class CreateNoteCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidCreateNoteCommand_ReturnsNoteId()
    {
        // Arrange
        var command = MockCommandQueryHelper.CreateNoteCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new CreateNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Note.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            MappedHelperResponse<NoteResponse, Domain.Models.Note.Note> note = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Notes.Count(u => u.Id == note.Entity.Id).Should().Be(1);
        }
    }
}