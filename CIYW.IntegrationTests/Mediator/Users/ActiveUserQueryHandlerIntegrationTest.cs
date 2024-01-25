using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Users.Handlers;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Users;

[TestFixture]
public class ActiveUserQueryHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockUserId, true)
{
	[Test]
	public async Task Handle_ValidActiveUserQuery_ReturnsUser()
	{
		// Arrange
		ActiveUserQuery command = new ActiveUserQuery();
            
		using (var scope = this.testApplicationFactory.Services.CreateScope())
		{
			this.SetClaims(scope);
			
			var handler = new ActiveUserQueryHandler(
				scope.ServiceProvider.GetRequiredService<IReadGenericRepository<ActiveUser>>(),
				scope.ServiceProvider.GetRequiredService<IMapper>(),
				scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
				scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
			);

			// Act
			MappedHelperResponse<ActiveUserResponse, ActiveUser> result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.MappedEntity.UserId.Should().Be(InitConst.MockUserId);
		}
	}
	
	[Test]
	public async Task Handle_InvalidActiveUserQuery_ReturnsException()
	{
		// Arrange
		ActiveUserQuery command = new ActiveUserQuery();
            
		using (var scope = this.testApplicationFactory.Services.CreateScope())
		{
			this.SetClaims(scope, InitConst.MockAuthUserId);
			
			var handler = new ActiveUserQueryHandler(
				scope.ServiceProvider.GetRequiredService<IReadGenericRepository<ActiveUser>>(),
				scope.ServiceProvider.GetRequiredService<IMapper>(),
				scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
				scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
			);

			// Act
			// Act
			await TestUtilities.Handle_InvalidCommand<ActiveUserQuery, MappedHelperResponse<ActiveUserResponse, ActiveUser>, LoggerException>(
				handler, 
				command, 
				String.Format(
					ErrorMessages.EntityWithIdNotFound,
					nameof(ActiveUser), 
					null));
		}
	}
}

