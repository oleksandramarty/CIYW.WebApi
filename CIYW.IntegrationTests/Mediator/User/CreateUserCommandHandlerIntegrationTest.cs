using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Auth.Handlers;
using CIYW.UnitTests;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.User;
[TestFixture]
public class CreateUserCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidCreateUserCommand_ReturnsUser()
    {
        // Arrange
        var command = MockHelper.CreateCreateUserCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new CreateUserCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<UserManager<Domain.Models.User.User>>()
            );

            // Act
            Domain.Models.User.User result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBe(null);
                
            this.dbContext.Users.Count(u => u.Id == result.Id).Should().Be(1);
            this.dbContext.UserLogins.Count(ul => ul.UserId == result.Id).Should().Be(3);
            this.dbContext.UserBalances.Count(ub => ub.UserId == result.Id).Should().Be(1);
            this.dbContext.UserRoles.Count(u => u.UserId == result.Id).Should().Be(1);
        }
    }
}