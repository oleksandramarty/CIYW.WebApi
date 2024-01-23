using AutoMapper;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Invoices;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Invoices.Handlers;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoices;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Invoices;

[TestFixture]
public class UserMonthlyInvoicesQueryHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateUserMonthlyInvoicesQuery()
    {
        yield return new TestCaseData("MockAdminUserId", -1, -1, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", -1, -1, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", -1, -1, false, InitConst.MockAuthUserId, 5);
        
        
        yield return new TestCaseData("MockAdminUserId", 1, 5, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", 1, 5, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", 1, 5, false, InitConst.MockAuthUserId, 5);
        
        
        yield return new TestCaseData("MockAdminUserId", 2, 5, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", 2, 5, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", 2, 5, false, InitConst.MockAuthUserId, 5);
        
        
        yield return new TestCaseData("MockAdminUserId", 1, 10, false, InitConst.MockAdminUserId, 10);
        yield return new TestCaseData("MockUserId", 1, 10, false, InitConst.MockUserId, 10);
        yield return new TestCaseData("MockAuthUserId", 1, 10, false, InitConst.MockAuthUserId, 10);
        
        
        yield return new TestCaseData("MockAdminUserId", 2, 10, false, InitConst.MockAdminUserId, 10);
        yield return new TestCaseData("MockUserId", 2, 10, false, InitConst.MockUserId, 10);
        yield return new TestCaseData("MockAuthUserId", 2, 10, false, InitConst.MockAuthUserId, 10);
        
        
        yield return new TestCaseData("MockAdminUserId", 3, 10, false, InitConst.MockAdminUserId, 10);
        yield return new TestCaseData("MockUserId", 3, 10, false, InitConst.MockUserId, 6);
        yield return new TestCaseData("MockAuthUserId", 3, 10, false, InitConst.MockAuthUserId, 10);   
        
        
        yield return new TestCaseData("MockAdminUserId", 4, 10, false, InitConst.MockAdminUserId, 10);
        yield return new TestCaseData("MockUserId", 4, 10, false, InitConst.MockUserId, 0);
        yield return new TestCaseData("MockAuthUserId", 6, 10, false, InitConst.MockAuthUserId, 1);
        
        
        yield return new TestCaseData("MockAdminUserId", 1, 1, true, InitConst.MockAdminUserId, 76);
        yield return new TestCaseData("MockUserId", 1, 1, true, InitConst.MockUserId, 26);
        yield return new TestCaseData("MockAuthUserId", 1, 1, true, InitConst.MockAuthUserId, 51);
    }
    
    [Test, TestCaseSource(nameof(CreateUserMonthlyInvoicesQuery))]
    public async Task Handle_ValidUserMonthlyInvoicesQuery_ReturnsImageId(
        string userName,
        int pageNumber,
        int pageSize,
        bool isFull, 
        Guid? userId,
        int expectedCount)
    {
        // Arrange
        UserMonthlyInvoicesQuery query = new UserMonthlyInvoicesQuery
        {
            Paginator = new Paginator
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IsFull = isFull
            },
            UserId = userId
        };

        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UserMonthlyInvoicesQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IMediator>()
            );

            // Act
            ListWithIncludeHelper<InvoiceResponse> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Entities.Should().NotBeNull();
            result.Entities.Count.Should().Be(expectedCount);
        }
    }
}