using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Auth.Handlers;
using CIYW.Mediator.Mediator.Auth.Requests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Auth;

[TestFixture]
public class CheckTemporaryPasswordQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> CreateCheckTemporaryPasswordQueryTestCases()
    {   
        yield return new TestCaseData(true, InitConst.MockUserId);
        yield return new TestCaseData(false, InitConst.MockUserId);
        yield return new TestCaseData(true, InitConst.MockAuthUserId);
        yield return new TestCaseData(false, InitConst.MockAuthUserId);
        yield return new TestCaseData(true, InitConst.MockAdminUserId);
        yield return new TestCaseData(false, InitConst.MockAdminUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateCheckTemporaryPasswordQueryTestCases))]
    public async Task Handle_ValidCheckTemporaryPasswordQuery_ReturnsState(bool expected, Guid userId)
    {
        // Arrange
        CheckTemporaryPasswordQuery query = new CheckTemporaryPasswordQuery();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
            bool currentState = user.IsTemporaryPassword;
            user.IsTemporaryPassword = expected;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            
            this.SetClaims(scope, userId);
            
            var handler = new CheckTemporaryPasswordQueryHandler(
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            bool result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(expected);
            
            user.IsTemporaryPassword = currentState;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}