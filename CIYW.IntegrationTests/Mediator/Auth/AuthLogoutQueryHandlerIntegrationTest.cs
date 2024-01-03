using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Mediator.Mediatr.Auth.Handlers;
using CIYW.Mediator.Mediatr.Auth.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Auth;

[TestFixture]
public class AuthLogoutQueryHandlerIntegrationTest : CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidUserAuthLogout_RemovesToken()
    {
        // Arrange
        AuthLogoutQuery query = new AuthLogoutQuery();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new AuthLogoutQueryHandler(
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            dbContext.UserTokens
                .Count(ut => ut.UserId == InitConst.MockUserId).Should()
                .Be(0);
        }
    }
}