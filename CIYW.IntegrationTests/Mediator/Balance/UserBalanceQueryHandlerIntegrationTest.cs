using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediatr.Balance.Handlers;
using CIYW.Mediator.Mediatr.Balance.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Balance;

[TestFixture]
public class UserBalanceQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidUserBalanceQuery_ReturnsBalanceAmount()
    {
        // Arrange
        UserBalanceQuery query = new UserBalanceQuery();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new UserBalanceQueryHandler(
                scope.ServiceProvider.GetRequiredService<IGenericRepository<UserBalance>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            decimal result = await handler.Handle(query, CancellationToken.None);
            UserBalance userBalance = dbContext.UserBalances.FirstOrDefault(ub => ub.UserId == InitConst.MockUserId);

            // Assert
            userBalance.Should().NotBeNull();
            userBalance.Should().BeEquivalentTo(new { Amount = result });
        }
    }
}