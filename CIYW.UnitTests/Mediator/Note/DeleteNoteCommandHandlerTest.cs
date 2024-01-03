using AutoMapper;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Invoice;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Note.Handlers;
using CIYW.Mediator.Mediator.Note.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Note;

[TestClass]
public class DeleteNoteCommandHandlerTest
{
    private readonly Mock<IGenericRepository<Domain.Models.Note.Note>> noteRepositoryMock;
    private readonly Mock<ICurrentUserProvider> currentUserProviderMock;
    private readonly Mock<IEntityValidator> entityValidatorMock;
    
    private readonly DeleteNoteCommandHandler handler;

    private readonly Domain.Models.Note.Note note;
    private readonly Invoice invoice;
    
    public DeleteNoteCommandHandlerTest()
    {
        this.invoice =
            MockHelper.GetMockInvoice(InitConst.MockUserId, InitConst.CategoryOtherId, InitConst.CurrencyUsdId);
        this.note = MockHelper.GetMockNote(InitConst.MockUserId, this.invoice.Id);
        this.invoice.Id = this.note.Id;
            
        this.noteRepositoryMock = new Mock<IGenericRepository<Domain.Models.Note.Note>>();
        this.currentUserProviderMock = new Mock<ICurrentUserProvider>();
        this.entityValidatorMock = new Mock<IEntityValidator>();

        this.noteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(this.note);
        
        this.currentUserProviderMock.Setup(r => r.GetUserIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(InitConst.MockUserId);
            
        this.handler = new DeleteNoteCommandHandler(
            this.noteRepositoryMock.Object,
            this.currentUserProviderMock.Object,
            this.entityValidatorMock.Object
        );
    }
    
    [TestMethod]
    public async Task Handle_ValidQuery_RemovesNote()
    {
        // Arrange
        DeleteNoteCommand command = new DeleteNoteCommand(this.note.Id);
        
        // Act
        await this.handler.Handle(command, CancellationToken.None);
        
        // Assert
        this.noteRepositoryMock.Verify(um => um.DeleteAsync(
            It.Is<Guid>(n => n == this.note.Id), It.IsAny<CancellationToken>()), Times.Once);
        this.currentUserProviderMock.Verify(up => up.GetUserIdAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}