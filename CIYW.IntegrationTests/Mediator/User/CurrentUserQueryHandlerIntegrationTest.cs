using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Auth.Handlers;
using CIYW.Mediator.Users.Handlers;
using CIYW.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using CIYW.UnitTests;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.User;

[TestFixture]
public class CurrentUserQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
        [Test]
        public async Task Handle_ValidCurrentUserQuery_ReturnsCurrentUser()
        {
            // Arrange
            var query = new CurrentUserQuery();
            
            using (var scope = this.testApplicationFactory.Services.CreateScope())
            {
                var handler = new CurrentUserQueryHandler(
                    scope.ServiceProvider.GetRequiredService<IMapper>(),
                    scope.ServiceProvider.GetRequiredService<IMediator>(),
                    scope.ServiceProvider.GetRequiredService<IReadGenericRepository<Domain.Models.User.User>>(),
                    scope.ServiceProvider.GetRequiredService<IReadGenericRepository<IdentityUserRole<Guid>>>(),
                    scope.ServiceProvider.GetRequiredService<IReadGenericRepository<Role>>(),
                    scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                    scope.ServiceProvider.GetRequiredService<IEntityValidator>()
                );

                // Act
                CurrentUserResponse result = await handler.Handle(query, CancellationToken.None);

                // Assert
                result.Should().NotBe(null);
                
                this.dbContext.Users.Count(u => u.Id == result.Id).Should().Be(1);
                this.dbContext.UserLogins.Count(ul => ul.UserId == result.Id).Should().Be(3);
                this.dbContext.UserBalances.Count(ub => ub.UserId == result.Id).Should().Be(1);
                this.dbContext.UserRoles.Count(u => u.UserId == result.Id).Should().Be(1);
            }
        }
}