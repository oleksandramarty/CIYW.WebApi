using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoices.Handlers;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Models.Responses.Invoices;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Invoices;

[TestFixture]
public class UpdateInvoiceCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> ValidUpdateInvoiceTestCases()
    {
        yield return new TestCaseData(1000.0m, InitConst.CategoryOtherId, InitConst.CurrencyUsdId, InvoiceTypeEnum.EXPENSE);
        yield return new TestCaseData(1500.0m, InitConst.CategorySalaryId, InitConst.CurrencyUsdId, InvoiceTypeEnum.INCOME);
        yield return new TestCaseData(900.0m, InitConst.CategoryOtherId, InitConst.CurrencyUsdId, InvoiceTypeEnum.EXPENSE);
        yield return new TestCaseData(2500.0m, InitConst.CategorySalaryId, InitConst.CurrencyUsdId, InvoiceTypeEnum.INCOME);
    }

    [Test, TestCaseSource(nameof(ValidUpdateInvoiceTestCases))]
    public async Task Handle_ValidUpdateInvoiceCommand_ReturnsUpdateInvoiceAndUpdateBalance(
        decimal amount, 
        Guid categoryId, 
        Guid currencyId, 
        InvoiceTypeEnum invoiceType)
    {
        // Arrange
        UserBalance expectedBalance = null;
        
        UpdateInvoiceCommand command = MockCommandQueryHelper.CreateUpdateInvoiceCommand(
            amount, categoryId, currencyId, DateTime.UtcNow, invoiceType);

        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            Domain.Models.Invoices.Invoice invoice = dbContext.Invoices.FirstOrDefault(i => i.UserId == InitConst.MockUserId);
            command.Id = invoice.Id;

            expectedBalance = dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId);
            
            expectedBalance.UpdateInvoice(invoice.Type, invoice.Amount, invoiceType, amount);

            var handler = new UpdateInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Invoices.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>());

            // Act
            MappedHelperResponse<InvoiceResponse, Domain.Models.Invoices.Invoice> result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Domain.Models.Invoices.Invoice updatedInvoice = dbContext.Invoices.FirstOrDefault(i => i.Id == invoice.Id && i.UserId == InitConst.MockUserId);
            updatedInvoice.Should().NotBeNull();

            updatedInvoice.Amount.Should().Be(amount);
            updatedInvoice.CategoryId.Should().Be(result.Entity.CategoryId);
            updatedInvoice.CurrencyId.Should().Be(result.Entity.CurrencyId);
            updatedInvoice.Type.Should().Be(result.Entity.Type);
            dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId).Amount.Should().Be(expectedBalance.Amount);
        }
    }
    
    [Test]
    public async Task Handle_InvalidUpdateInvoiceCommand_ReturnsExceptionNotFound()
    {
        // Arrange
        UpdateInvoiceCommand command = MockCommandQueryHelper.CreateUpdateInvoiceCommand(
            1000.0m, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, InvoiceTypeEnum.EXPENSE);
        command.Id = Guid.NewGuid();
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UpdateInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Invoices.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>());

            // Act
            await TestUtilities.Handle_InvalidCommand<UpdateInvoiceCommand, MappedHelperResponse<InvoiceResponse, Domain.Models.Invoices.Invoice>, LoggerException>(
                handler, 
                command, 
                String.Format(ErrorMessages.EntityWithIdNotFound, nameof(Domain.Models.Invoices.Invoice), command.Id));
        }
    }
    
    [Test]
    public async Task Handle_InvalidUpdateInvoiceCommand_ReturnsExceptionForbidden()
    {
        // Arrange
        UpdateInvoiceCommand command = MockCommandQueryHelper.CreateUpdateInvoiceCommand(
            1000.0m, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, InvoiceTypeEnum.EXPENSE);
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            Domain.Models.Invoices.Invoice invoice = dbContext.Invoices.FirstOrDefault(i => i.UserId == InitConst.MockAuthUserId);

            command.Id = invoice.Id;
            
            var handler = new UpdateInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Invoices.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>());

            // Act
            await TestUtilities.Handle_InvalidCommand<UpdateInvoiceCommand, MappedHelperResponse<InvoiceResponse, Domain.Models.Invoices.Invoice>, LoggerException>(
                handler, 
                command, 
                ErrorMessages.Forbidden);
        }
    }
}