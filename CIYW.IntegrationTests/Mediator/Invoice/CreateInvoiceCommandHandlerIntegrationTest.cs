using AutoMapper;
using CIYW.Const.Enum;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediatr.Invoice.Handlers;
using CIYW.Mediator.Mediatr.Invoice.Requests;
using CIYW.Mediator.Mediatr.Note.Request;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Invoice;

[TestFixture]
public class CreateInvoiceCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> ValidCreateInvoiceTestCases()
    {
        yield return new TestCaseData(1000.0m, InitConst.CategoryOtherId, InitConst.CurrencyUsdId, InvoiceTypeEnum.Expense, null, null);
        yield return new TestCaseData(1500.0m, InitConst.CategorySalaryId, InitConst.CurrencyUsdId, InvoiceTypeEnum.Income, null, null);
        yield return new TestCaseData(900.0m, InitConst.CategoryOtherId, InitConst.CurrencyUsdId, InvoiceTypeEnum.Expense, "TestNoteName", "TestNoteBody");
        yield return new TestCaseData(2500.0m, InitConst.CategorySalaryId, InitConst.CurrencyUsdId, InvoiceTypeEnum.Income, "TestNoteName", "TestNoteBody");
    }

    [Test, TestCaseSource(nameof(ValidCreateInvoiceTestCases))]
    public async Task Handle_ValidCreateInvoiceCommand_ReturnsGuidAndUpdateBalance(
        decimal amount, 
        Guid categoryId, 
        Guid currencyId, 
        InvoiceTypeEnum invoiceType,
        string noteName,
        string noteBody)
    {
        // Arrange
        UserBalance expectedBalance = null;
        
        CreateOrUpdateNoteCommand noteCommand = null;
        if (noteName.NotNullOrEmpty() && noteBody.NotNullOrEmpty())
        {
            noteCommand = MockCommandQueryHelper.CreateCreateOrUpdateNoteCommand(noteName, noteBody);
        }
        CreateInvoiceCommand command = MockCommandQueryHelper.CreateCreateInvoiceCommand(
            amount, categoryId, currencyId, DateTime.UtcNow, invoiceType, noteCommand);

        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            expectedBalance = dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId);
            
            expectedBalance.AddInvoice(invoiceType, amount);

            var handler = new CreateInvoiceCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ITransactionRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>());

            // Act
            Guid result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            dbContext.Invoices.Count(i => i.Id == result && i.UserId == InitConst.MockUserId).Should().Be(1);
            dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId).Amount.Should().Be(expectedBalance.Amount);

            if (noteCommand != null)
            {
                dbContext.Notes.Count(n => n.InvoiceId.HasValue && n.InvoiceId.Value == result).Should().Be(1);
            }
        }
    }
}