using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Invoices.Handlers;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Invoices;

[TestFixture]
public class DeleteInvoiceCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> ValidCreateInvoiceTestCases()
    {
        yield return new TestCaseData();
        yield return new TestCaseData();
        yield return new TestCaseData();
        yield return new TestCaseData();
        yield return new TestCaseData();
        yield return new TestCaseData();
        yield return new TestCaseData();
        yield return new TestCaseData();
    }

    [Test, TestCaseSource(nameof(ValidCreateInvoiceTestCases))]
    public async Task Handle_ValidDeleteInvoiceCommand_DeleteInvoiceAndUpdateBalance()
    {
        // Arrange
        UserBalance expectedBalance = null;

        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Invoices.Invoice invoice = dbContext.Invoices.FirstOrDefault(i => i.UserId == InitConst.MockUserId);

            expectedBalance = dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId);
            
            expectedBalance.DeleteInvoice(invoice.Type, invoice.Amount);

            var handler = new DeleteInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Invoices.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>());

            // Act
            await handler.Handle(new DeleteInvoiceCommand(invoice.Id), CancellationToken.None);
            
            // Assert
            dbContext.Invoices.Count(i => i.Id == invoice.Id && i.UserId == InitConst.MockUserId).Should().Be(0);
            dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId).Amount.Should().Be(expectedBalance.Amount);
        }
    }
    
    [Test]
    public async Task Handle_InvalidDeleteInvoiceCommand_ReturnsExceptionNotFound()
    {
        // Arrange
        Guid invoiceId = Guid.NewGuid();
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new DeleteInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Invoices.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>());

            // Act
            await TestUtilities.Handle_InvalidCommand<DeleteInvoiceCommand, LoggerException>(
                handler, 
                new DeleteInvoiceCommand(invoiceId), 
                String.Format(ErrorMessages.EntityWithIdNotFound, nameof(Domain.Models.Invoices.Invoice), invoiceId));
        }
    }
    
    [Test]
    public async Task Handle_InvalidDeleteInvoiceCommand_ReturnsExceptionForbidden()
    {
        // Arrange
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Invoices.Invoice invoice = dbContext.Invoices.FirstOrDefault(i => i.UserId == InitConst.MockAuthUserId);
            
            var handler = new DeleteInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Invoices.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>());

            // Act
            await TestUtilities.Handle_InvalidCommand<DeleteInvoiceCommand, LoggerException>(
                handler, 
                new DeleteInvoiceCommand(invoice.Id), 
                ErrorMessages.Forbidden);
        }
    }
}