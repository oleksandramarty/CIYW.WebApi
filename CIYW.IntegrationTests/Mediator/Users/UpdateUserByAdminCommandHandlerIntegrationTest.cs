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
public class UpdateUserByAdminCommandHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateValidUpdateUserByAdminCommandTestCases()
    {   
        yield return new TestCaseData("LastName",  "FirstName", "Patronymic", "MyLogin123", "myemail123123@mail.com", "55555555555", true, "zcbm13579", true, true, InitConst.FreeTariffId, InitConst.CurrencyUsdId, InitConst.MockUserId);
        yield return new TestCaseData("LastName",  "FirstName", "Patronymic", "MyLogin123", "myemail123123@mail.com", "55555555555", true, "zcbm13579", false, true, InitConst.FreeTariffId, InitConst.CurrencyUsdId, InitConst.MockUserId);
        yield return new TestCaseData("LastName",  "FirstName", "Patronymic", "MyLogin123", "myemail123123@mail.com", "55555555555", true, "zcbm13579", true, false, InitConst.FreeTariffId, InitConst.CurrencyUsdId, InitConst.MockUserId);
        yield return new TestCaseData("LastName",  "FirstName", "Patronymic", "MyLogin123", "myemail123123@mail.com", "55555555555", true, "zcbm13579", false, false, InitConst.FreeTariffId, InitConst.CurrencyUsdId, InitConst.MockUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateValidUpdateUserByAdminCommandTestCases))]
    public async Task Handle_ValidUpdateUserByAdminCommand_ReturnsUser(
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
        Guid currencyId,
        Guid userId
    )
    {
        // Arrange
        UpdateUserByAdminCommand command = new UpdateUserByAdminCommand(
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
            currencyId)
        {
            Id = userId
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var handler = new UpdateUserByAdminCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IElasticSearchRepository>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<UserResponse, User> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
                
            dbContext.Users.Any(x => 
                x.Id == userId &&
                x.LastName == lastName &&
                x.FirstName == firstName &&
                x.Patronymic == patronymic &&
                x.Login == login &&
                x.Email == email &&
                x.PhoneNumber == phone &&
                x.IsTemporaryPassword == isTemporaryPassword &&
                x.IsBlocked == isBlocked &&
                x.CurrencyId == currencyId &&
                x.TariffId == tariffId).Should().Be(true);
        }
    }
}