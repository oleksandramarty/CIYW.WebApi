using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Users.Handlers;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Users;

[TestFixture]
public class CreateUserByAdminCommandHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateValidCreateUserByAdminCommandTestCases()
    {   
        yield return new TestCaseData("LastName",  "FirstName", "Patronymic", "MyLogin123", "myemail123123@mail.com", "55555555555", true, "zcbm13579", true, true, InitConst.FreeTariffId, InitConst.CurrencyUsdId);
        yield return new TestCaseData("LastName12",  "FirstName12", null, "MyLogin12312", "myemail12312312@mail.com", "77777777777", true, "zcbm13579", true, true, InitConst.FreeTariffId, InitConst.CurrencyUsdId);
    }
    
    [Test, TestCaseSource(nameof(CreateValidCreateUserByAdminCommandTestCases))]
    public async Task Handle_ValidCreateUserByAdminCommand_ReturnsUser(
        string lastName,
        string firstName,
        string patronymic,
        string login,
        string email,
        string phone,
        bool isAgree,
        string password,
        bool isTemporaryPassword,
        bool isBlocked,
        Guid tariffId,
        Guid currencyId
    )
    {
        // Arrange
        CreateUserByAdminCommand command = new CreateUserByAdminCommand(
            lastName,
            firstName,
            patronymic,
            login,
            email,
            phone,
            email,
            isAgree,
            password,
            password,
            true,
            isTemporaryPassword,
            isBlocked,
            tariffId,
            currencyId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new CreateUserByAdminCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                scope.ServiceProvider.GetRequiredService<IElasticSearchRepository>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<UserResponse, User> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
                
            dbContext.Users.Count(u => u.Id == result.Entity.Id).Should().Be(1);
            dbContext.UserLogins.Count(ul => ul.UserId == result.Entity.Id).Should().Be(3);
            dbContext.UserBalances.Count(ub => ub.UserId == result.Entity.Id).Should().Be(1);
            dbContext.UserRoles.Count(u => u.UserId == result.Entity.Id).Should().Be(1);
        }
    }
}