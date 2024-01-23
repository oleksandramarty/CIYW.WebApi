using AutoMapper;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Invoices;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Notes.Handlers;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Notes;

[TestClass]
public class UpdateNoteCommandHandlerTest
{
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IGenericRepository<Domain.Models.Notes.Note>> noteRepositoryMock;
    private readonly Mock<ICurrentUserProvider> currentUserProviderMock;
    private readonly Mock<IEntityValidator> entityValidatorMock;
    
    private readonly UpdateNoteCommandHandler handler;

    private readonly Domain.Models.Notes.Note note;
    private readonly Invoice invoice;
    
    public UpdateNoteCommandHandlerTest()
    {
        this.mapperMock = new Mock<IMapper>();
        this.invoice =
            MockHelper.GetMockInvoice(InitConst.MockUserId, InitConst.CategoryOtherId, InitConst.CurrencyUsdId);
        this.note = MockHelper.GetMockNote(InitConst.MockUserId, this.invoice.Id);
        this.invoice.Id = this.note.Id;
        this.mapperMock.Setup(m => m.Map<CreateOrUpdateNoteCommand, Domain.Models.Notes.Note>(
                It.IsAny<CreateOrUpdateNoteCommand>(), 
                It.IsAny<Domain.Models.Notes.Note>(),
                It.IsAny<Action<IMappingOperationOptions<CreateOrUpdateNoteCommand, Domain.Models.Notes.Note>>>()))
            .Returns(this.note);
            
        this.noteRepositoryMock = new Mock<IGenericRepository<Domain.Models.Notes.Note>>();
        this.currentUserProviderMock = new Mock<ICurrentUserProvider>();
        this.entityValidatorMock = new Mock<IEntityValidator>();

        this.noteRepositoryMock.Setup(r => r.GetByIdAsync(
                It.Is<Guid>(u => u == this.note.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(this.note);
        
        this.currentUserProviderMock.Setup(r => r.GetUserIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(InitConst.MockUserId);
            
        this.handler = new UpdateNoteCommandHandler(
            this.mapperMock.Object,
            this.noteRepositoryMock.Object,
            this.currentUserProviderMock.Object,
            this.entityValidatorMock.Object
        );
    }
    
    [TestMethod]
    public async Task Handle_ValidQuery_ReturnsNoteId()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Id = this.note.Id;
        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.IsNotNull(result);
        result.Entity.Id.Should().Be(this.note.Id);
        this.noteRepositoryMock.Verify(um => um.UpdateAsync(
            It.Is<Domain.Models.Notes.Note>(n => n.Name == this.note.Name &&
                                                 n.Body == this.note.Body), It.IsAny<CancellationToken>()), Times.Once);
        this.currentUserProviderMock.Verify(up => up.GetUserIdAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}