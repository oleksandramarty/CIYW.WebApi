using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Invoices;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Invoices.Handlers;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Invoices;

[TestFixture]
public class UserInvoicesQueryHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateUserInvoicesQuery()
    {
        DateTime date_01 = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        DateTime date_15 = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 15);
        DateTime date_20 = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 20);
        
        yield return new TestCaseData("MockAdminUserId", date_01, date_15, -1, -1, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", date_01, date_15, -1, -1, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", date_01, date_15, -1, -1, false, InitConst.MockAuthUserId, 5);
        yield return new TestCaseData("MockAdminUserId", date_15, date_20, -1, -1, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", date_15, date_20, -1, -1, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", date_15, date_20, -1, -1, false, InitConst.MockAuthUserId, 5);
        yield return new TestCaseData("", date_15, date_20, -1, -1, false, null, 5);
        yield return new TestCaseData("", date_15, date_20, -1, -1, false, null, 5);
        

        yield return new TestCaseData("MockAdminUserId", date_01, date_15, 1, 5, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", date_01, date_15, 1, 5, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", date_01, date_15, 1, 5, false, InitConst.MockAuthUserId, 5);
        yield return new TestCaseData("MockAdminUserId", date_15, date_20, 1, 5, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", date_15, date_20, 1, 5, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", date_15, date_20, 1, 5, false, InitConst.MockAuthUserId, 5);
        yield return new TestCaseData("", date_15, date_20, 1, 5, false, null, 5);
        yield return new TestCaseData("", date_15, date_20, 1, 5, false, null, 5);
        
        
        yield return new TestCaseData("MockAdminUserId", date_01, date_15, 2, 5, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", date_01, date_15, 2, 5, false, InitConst.MockUserId, 5);
        yield return new TestCaseData("MockAuthUserId", date_01, date_15, 2, 5, false, InitConst.MockAuthUserId, 5);
        yield return new TestCaseData("MockAdminUserId", date_15, date_20, 2, 5, false, InitConst.MockAdminUserId, 5);
        yield return new TestCaseData("MockUserId", date_15, date_20, 2, 5, false, InitConst.MockUserId, 1);
        yield return new TestCaseData("MockAuthUserId", date_15, date_20, 2, 5, false, InitConst.MockAuthUserId, 5);
        yield return new TestCaseData("", date_15, date_20, 2, 5, false, null, 5);
        yield return new TestCaseData("", date_15, date_20, 2, 5, false, null, 5);
        
        
        yield return new TestCaseData("MockAdminUserId", date_01, date_15, 1, 10, false, InitConst.MockAdminUserId, 10);
        yield return new TestCaseData("MockUserId", date_01, date_15, 1, 10, false, InitConst.MockUserId, 10);
        yield return new TestCaseData("MockAuthUserId", date_01, date_15, 1, 10, false, InitConst.MockAuthUserId, 10);
        yield return new TestCaseData("MockAdminUserId", date_15, date_20, 1, 10, false, InitConst.MockAdminUserId, 10);
        yield return new TestCaseData("MockUserId", date_15, date_20, 1, 10, false, InitConst.MockUserId, 6);
        yield return new TestCaseData("MockAuthUserId", date_15, date_20, 1, 10, false, InitConst.MockAuthUserId, 10);
        yield return new TestCaseData("", date_15, date_20, 1, 10, false, null, 10);
        yield return new TestCaseData("", date_15, date_20, 1, 10, false, null, 10);
        
        
        yield return new TestCaseData("MockAdminUserId", date_01, date_15, 1, 1, true, InitConst.MockAdminUserId, 46);
        yield return new TestCaseData("MockUserId", date_01, date_15, 1, 1, true, InitConst.MockUserId, 16);
        yield return new TestCaseData("MockAuthUserId", date_01, date_15, 1, 1, true, InitConst.MockAuthUserId, 31);
        yield return new TestCaseData("MockAdminUserId", date_15, date_20, 1, 1, true, InitConst.MockAdminUserId, 18);
        yield return new TestCaseData("MockUserId", date_15, date_20, 1, 1, true, InitConst.MockUserId, 6);
        yield return new TestCaseData("MockAuthUserId", date_15, date_20, 1, 1, true, InitConst.MockAuthUserId, 12);
        yield return new TestCaseData("", date_01, date_15, 1, 1, true, null, 93);
        yield return new TestCaseData("", date_15, date_20, 1, 1, true, null, 36);
        
        
        yield return new TestCaseData("MockAdminUserId", date_01, null, 1, 1, true, InitConst.MockAdminUserId, 76);
        yield return new TestCaseData("MockUserId", date_01, null, 1, 1, true, InitConst.MockUserId, 26);
        yield return new TestCaseData("MockAuthUserId", date_01, null, 1, 1, true, InitConst.MockAuthUserId, 51);
        yield return new TestCaseData("MockAdminUserId", date_15, null, 1, 1, true, InitConst.MockAdminUserId, 33);
        yield return new TestCaseData("MockUserId", date_15, null, 1, 1, true, InitConst.MockUserId, 11);
        yield return new TestCaseData("MockAuthUserId", date_15, null, 1, 1, true, InitConst.MockAuthUserId, 22);
        yield return new TestCaseData("", date_01, null, 1, 1, true, null, 153);
        yield return new TestCaseData("", date_15, null, 1, 1, true, null, 66);
        
        
        yield return new TestCaseData("MockAdminUserId", null, date_15, 1, 1, true, InitConst.MockAdminUserId, 46);
        yield return new TestCaseData("MockUserId", null, date_15, 1, 1, true, InitConst.MockUserId, 16);
        yield return new TestCaseData("MockAuthUserId", null, date_15, 1, 1, true, InitConst.MockAuthUserId, 31);
        yield return new TestCaseData("MockAdminUserId", null, date_20, 1, 1, true, InitConst.MockAdminUserId, 61);
        yield return new TestCaseData("MockUserId", null, date_20, 1, 1, true, InitConst.MockUserId, 21);
        yield return new TestCaseData("MockAuthUserId", null, date_20, 1, 1, true, InitConst.MockAuthUserId, 41);
        yield return new TestCaseData("", null, date_15, 1, 1, true, null, 93);
        yield return new TestCaseData("", null, date_20, 1, 1, true, null, 123);
        
        
        yield return new TestCaseData("MockAdminUserId", null, null, 1, 1, true, InitConst.MockAdminUserId, 76);
        yield return new TestCaseData("MockUserId", null, null, 1, 1, true, InitConst.MockUserId, 26);
        yield return new TestCaseData("MockAuthUserId", null, null, 1, 1, true, InitConst.MockAuthUserId, 51);
        yield return new TestCaseData("MockAdminUserId", null, null, 1, 1, true, InitConst.MockAdminUserId, 76);
        yield return new TestCaseData("MockUserId", null, null, 1, 1, true, InitConst.MockUserId, 26);
        yield return new TestCaseData("MockAuthUserId", null, null, 1, 1, true, InitConst.MockAuthUserId, 51);
        yield return new TestCaseData("", null, null, 1, 1, true, null, 153);
    }
    
    [Test, TestCaseSource(nameof(CreateUserInvoicesQuery))]
    public async Task Handle_ValidUserInvoicesQuery_ReturnsImageId(
        string userName,
        DateTime? dateFrom, 
        DateTime? dateTo,
        int pageNumber,
        int pageSize,
        bool isFull, 
        Guid? userId,
        int expectedCount)
    {
        // Arrange
        UserInvoicesQuery query = new UserInvoicesQuery(new BaseFilterQuery
        {
            UserId = userId,
            Ids = new BaseIdsListQuery { Ids = new List<Guid> { InitConst.MockUserId, InitConst.MockAdminUserId, InitConst.AdminUserId } },
            Paginator = new Paginator { PageNumber = pageNumber, PageSize = pageSize, IsFull = isFull },
            Sort = new BaseSortableQuery { Column = "Created", Direction = "desc" },
            DateRange = new BaseDateRangeQuery { DateFrom = dateFrom?.ToUtc(), DateTo = dateTo?.ToUtc() }
        });
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UserInvoicesQueryHandler(
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IReadGenericRepository<Invoice>>()
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