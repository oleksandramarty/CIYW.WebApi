using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoice.Handlers;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Responses.Invoice;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using LoggerException = CIYW.Kernel.Exceptions.LoggerException;

namespace CIYW.IntegrationTests.Mediator.Invoice;

[TestFixture]
public class GetInvoiceByIdQueryHandlerIntegrationTest: CommonIntegrationTestSetup
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
            if (errorMessage.NotNullOrEmpty())
            {
                await TestUtilities.Handle_InvalidCommand<GetInvoiceByIdQuery, MappedHelperResponse<BalanceInvoiceResponse, Domain.Models.Invoice.Invoice>, LoggerException>(
                    handler, query, errorMessage);
            }
            else
            {
                MappedHelperResponse<BalanceInvoiceResponse, Domain.Models.Invoice.Invoice> result = await handler.Handle(query, CancellationToken.None);
            
                // Assert
                dbContext.Invoices.Count(i => i.Id == invoice.Id).Should().Be(1);
                result.MappedEntity.Id.Should().Be(invoice.Id);
                result.MappedEntity.Name.Should().Be(invoice.Name);
                result.MappedEntity.Amount.Should().Be(invoice.Amount);
                result.MappedEntity.Type.Should().Be(invoice.Type);
            }
        }
    }
}