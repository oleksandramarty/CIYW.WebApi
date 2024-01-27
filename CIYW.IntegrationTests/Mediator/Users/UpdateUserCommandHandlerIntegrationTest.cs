using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Users.Handlers;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Users;

[TestFixture]
public class UpdateUserCommandHandlerIntegrationTest(): CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> CreateValidUpdateUserCommandTestCases()
    {   
        yield return new TestCaseData("LastName",  "FirstName", "Patronymic", "MyLogin123", "myemail123123@mail.com", "55555555555", true, "zcbm13579", InitConst.MockUserId);
        yield return new TestCaseData("LastName12",  "FirstName12", null, "MyLogin12312", "myemail12312312@mail.com", "77777777777", true, "zcbm13579", InitConst.MockAuthUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateValidUpdateUserCommandTestCases))]
    public async Task Handle_ValidUpdateUserCommand_ChangesPassword(
        string lastName, 
        string firstName, 
        string patronymic, 
        string login, 
        string email, 
        string phone, 
        bool isAgree, 
        string password,
        Guid userId
        )
    {
        // Arrange
        UpdateUserCommand command = new UpdateUserCommand(
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
            true);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            this.options.UpdateClaims(scope, userId);
            
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
            
            var handler = new UpdateUserCommandHandler(
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
                x.PhoneNumber == phone).Should().Be(true);
        }
    }
    
    private static IEnumerable<TestCaseData> CreateInvalidUpdateUserCommandTestCases()
    {   
        yield return new TestCaseData("zcbm135791", InitConst.MockUserId);
        yield return new TestCaseData("zcbm135791", InitConst.MockAuthUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateInvalidUpdateUserCommandTestCases))]
    public async Task Handle_InvalidUpdateUserCommand_ChangesPassword(string password, Guid userId)
    {
        // Arrange
        string email = $"{StringExtension.GenerateRandomString(10)}@mail.com";
        UpdateUserCommand command = new UpdateUserCommand(
            StringExtension.GenerateRandomString(10),
            StringExtension.GenerateRandomString(10),
            null,
            StringExtension.GenerateRandomString(10),
            email,
            StringExtension.GenerateRandomString(10),
            email,
            true,
            password,
            password,
            true);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            this.options.UpdateClaims(scope, userId);
            
            var handler = new UpdateUserCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IElasticSearchRepository>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Assert
            await TestUtilities.Handle_InvalidCommand<UpdateUserCommand, MappedHelperResponse<UserResponse, User>, LoggerException>(handler, command, ErrorMessages.WrongAuth);
        }
    }
}