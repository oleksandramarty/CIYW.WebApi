using AutoMapper;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Users.Handlers;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Users;

[TestFixture]
public class UserByIdQueryHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    [Test]
    public async Task Handle_ValidUserByIdQuery_ReturnsUser()
    {
        // Arrange
        UserByIdQuery query = new UserByIdQuery(InitConst.MockUserId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UserByIdQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMediator>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IReadGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IReadGenericRepository<Role>>(),
                scope.ServiceProvider.GetRequiredService<IReadGenericRepository<IdentityUserRole<Guid>>>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            MappedHelperResponse<UserResponse, User> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            result.Entity.Id.Should().Be(InitConst.MockUserId);
            result.MappedEntity.Id.Should().Be(InitConst.MockUserId);
        }
    }
}