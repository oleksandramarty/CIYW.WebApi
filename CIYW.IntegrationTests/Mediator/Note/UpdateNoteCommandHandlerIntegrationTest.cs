using AutoMapper;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Mediator.Mediatr.Note.Handlers;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Note;

[TestFixture]
public class UpdateNoteCommandHandlerIntegrationTest: CommonIntegrationTestSetup
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
            
            var handlerUpdate = new UpdateNoteCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Note.Note>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            Guid noteId = await handlerAdd.Handle(command, CancellationToken.None);
            command.Id = noteId;
            command.Name = "UpdatedName";
            Guid result = await handlerUpdate.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Notes.Count(u => u.Id == result && u.Name == command.Name).Should().Be(1);
        }
    }
}