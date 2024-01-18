using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Users.Handlers;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
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
                DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                
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
                UserResponse result = await handler.Handle(query, CancellationToken.None);

                // Assert
                result.Should().NotBe(null);
                
                dbContext.Users.Count(u => u.Id == result.Id).Should().Be(1);
                dbContext.UserLogins.Count(ul => ul.UserId == result.Id).Should().Be(3);
                dbContext.UserBalances.Count(ub => ub.UserId == result.Id).Should().Be(1);
                dbContext.UserRoles.Count(u => u.UserId == result.Id).Should().Be(1);
            }
        }
}