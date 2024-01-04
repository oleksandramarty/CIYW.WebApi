using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Invoice.Handlers;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Responses.Invoice;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Common;

public class AccessGrantedForAdminHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> ValidTestCases()
    {
        for (int i = 1; i <= 5; i++)
        {
            yield return new TestCaseData(InitConst.MockUserId, null);
        }
        for (int i = 1; i <= 5; i++)
        {
            yield return new TestCaseData(InitConst.MockAuthUserId, ErrorMessages.Forbidden);
        }
    }

    [Test, TestCaseSource(nameof(ValidTestCases))]
    public async Task Handle_ValidGetInvoiceByIdQuery_ReturnsBalanceInvoiceResponse(Guid userId, string errorMessage)
    {
        // Arrange
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            Domain.Models.Invoice.Invoice invoice = dbContext.Invoices.FirstOrDefault(ub => ub.UserId == userId);

            var handler = new GetInvoiceByIdQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IReadGenericRepository<Domain.Models.Invoice.Invoice>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
                );

            GetInvoiceByIdQuery query = new GetInvoiceByIdQuery(invoice.Id);
            
            // Act
            BalanceInvoiceResponse result = await handler.Handle(query, CancellationToken.None);
            
            // Assert
            dbContext.Invoices.Count(i => i.Id == invoice.Id).Should().Be(1);
            result.Id.Should().Be(invoice.Id);
            result.Name.Should().Be(invoice.Name);
            result.Amount.Should().Be(invoice.Amount);
            result.Type.Should().Be(invoice.Type);
        }
    }
}