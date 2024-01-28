using System.Text.RegularExpressions;
using AutoMapper;
using CIYW.Auth.Tokens;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Common;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Handlers;
using CIYW.Mediator.Mediator.Auth.Queries;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Auth;

[TestFixture]
public class RestorePasswordCommandHandlerIntegrationTest() : CommonIntegrationTestSetup(null)
{
    private static IEnumerable<TestCaseData> CreateValidRestorePasswordCommandTestCases()
    {   
        yield return new TestCaseData("john.doe", "myemail@mail.com", "12124567890", StringExtension.GenerateRandomPassword(10,20), InitConst.MockUserId);
        yield return new TestCaseData("anime.kit", "animekit@mail.com", "22334433221", StringExtension.GenerateRandomPassword(10,20), InitConst.MockAuthUserId);
        yield return new TestCaseData("admin.test", "admintest@mail.com", "44332255332", StringExtension.GenerateRandomPassword(10,20), InitConst.MockAdminUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateValidRestorePasswordCommandTestCases))]
    public async Task Handle_ValidRestorePasswordCommand_ChangesPassword(
        string login,
        string email,
        string phone,
        string pass,
        Guid userId
        )
    {
        // Arrange
        RestorePasswordCommand command = new RestorePasswordCommand
        {
            Login = login,
            Email = email,
            Phone = phone,
            Password = pass,
            ConfirmPassword = pass,
            Url = string.Empty
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            await this.RemoveRestorePasswordDataAsync(CancellationToken.None);
            
            RestorePassword restore = new RestorePassword
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                UserId = userId,
                Url = StringExtension.GenerateRandomString(50).ToUpper()
            };
            
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            command.Url = rgx.Replace($"{restore.Id}{restore.UserId}{restore.Url}", "");

            await dbContext.RestorePasswords.AddAsync(restore, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            
            var handler = new RestorePasswordCommandHandler(
                scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<RestorePassword>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );
            
            var loginHandler = new AuthLoginQueryHandler(
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<TokenGenerator>(),
                scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>()
            );

            // Act
            await handler.Handle(command, CancellationToken.None);
            await loginHandler.Handle(
                new AuthLoginQuery
                {
                    Login = login, 
                    Email = email, 
                    Phone = phone, 
                    Password = pass, 
                    RememberMe = false
                }, 
                CancellationToken.None);

            // Assert
            dbContext.UserTokens.Any(x => x.UserId == userId).Should().Be(true);
            
            await this.RemoveRestorePasswordDataAsync(CancellationToken.None);
        }
    }
    
    private static IEnumerable<TestCaseData> CreateInvalidRestorePasswordCommandTestCases()
    {   
        yield return new TestCaseData(ErrorMessages.NotFound, null, null, "john.doe", "myemail@mail.com", "12124567890", StringExtension.GenerateRandomPassword(10,20), InitConst.MockUserId);
        yield return new TestCaseData(ErrorMessages.AlreadyUsed, DateTime.UtcNow, DateTime.UtcNow.AddHours(-1), "john.doe", "myemail@mail.com", "12124567890", StringExtension.GenerateRandomPassword(10,20), InitConst.MockUserId);
        yield return new TestCaseData(ErrorMessages.AuthError, DateTime.UtcNow, null, "john.doe", "myemail@mail.com", "12124567890", "123", InitConst.MockUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateInvalidRestorePasswordCommandTestCases))]
    public async Task Handle_InvalidRestorePasswordCommand_ReturnsException(
        string expectedError,
        DateTime? created,
        DateTime? used,
        string login,
        string email,
        string phone,
        string pass,
        Guid userId
        )
    {
        // Arrange
        RestorePasswordCommand command = new RestorePasswordCommand
        {
            Login = login,
            Email = email,
            Phone = phone,
            Password = pass,
            ConfirmPassword = pass,
            Url = string.Empty
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            await this.RemoveRestorePasswordDataAsync(CancellationToken.None);

            if (created.HasValue)
            {
                RestorePassword restore = new RestorePassword
                {
                    Id = Guid.NewGuid(),
                    Created = created.Value,
                    UserId = userId,
                    Used = used,
                    Url = StringExtension.GenerateRandomString(50).ToUpper()
                };                
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                command.Url = rgx.Replace($"{restore.Id}{restore.UserId}{restore.Url}", "");
                
                await dbContext.RestorePasswords.AddAsync(restore, CancellationToken.None);
                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
            else
            {
                command.Url = StringExtension.GenerateRandomString(114);
            }
            
            var handler = new RestorePasswordCommandHandler(
                scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<RestorePassword>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<RestorePasswordCommand, LoggerException>(
                handler, 
                command, 
                expectedError,
                async () =>
                {
                    await this.RemoveRestorePasswordDataAsync(CancellationToken.None);
                });
        }
    }
}