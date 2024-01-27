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
using CIYW.Models.Responses.Auth;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Auth;

[TestFixture]
public class ChangePasswordCommandHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> CreateValidChangePasswordCommandTestCases()
    {   
        yield return new TestCaseData("zcbm13579", "zcbm13579123zc", InitConst.MockUserId);
        yield return new TestCaseData("zcbm13579", "zcbm13579123zc", InitConst.MockAuthUserId);
        yield return new TestCaseData("zcbm13579", "zcbm13579123zc", InitConst.MockAdminUserId);
    }
    
    [Test, TestCaseSource(nameof(CreateValidChangePasswordCommandTestCases))]
    public async Task Handle_ValidChangePasswordCommand_ChangesPassword(
        string oldPas,
        string newPass,
        Guid userId
        )
    {
        // Arrange
        ChangePasswordCommand command = new ChangePasswordCommand
        {
            OldPassword = oldPas,
            NewPassword = newPass,
            ConfirmationPassword = newPass
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            this.options.UpdateClaims(scope, userId);
            
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
            
            var handler = new ChangePasswordCommandHandler(
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
            
            this.options.UpdateClaims(scope, null);
            
            await loginHandler.Handle(
                new AuthLoginQuery(user.Login, user.Email, user.PhoneNumber, newPass, false), 
                CancellationToken.None);

            // Assert
            dbContext.UserTokens.Any(x => x.UserId == userId).Should().Be(true);
            
            await this.RemoveRestorePasswordDataAsync(CancellationToken.None);
        }
    }
    
    private static IEnumerable<TestCaseData> CreateInvalidChangePasswordCommandTestCases()
    {
        yield return new TestCaseData("zcbm13579", InitConst.MockUserId, ErrorMessages.WrongAuth);
        yield return new TestCaseData("zcbm13579", InitConst.MockAuthUserId, ErrorMessages.WrongAuth);
        yield return new TestCaseData("zcbm13579", InitConst.MockAdminUserId, ErrorMessages.WrongAuth);
    }
    
    [Test, TestCaseSource(nameof(CreateInvalidChangePasswordCommandTestCases))]
    public async Task Handle_InvalidChangePasswordCommand_ChangesPassword(
        string oldPas,
        Guid userId,
        string errorMessage
        )
    {
        // Arrange
        string newPass = StringExtension.GenerateRandomPassword(8, 10);
        ChangePasswordCommand command = new ChangePasswordCommand
        {
            OldPassword = oldPas,
            NewPassword = newPass,
            ConfirmationPassword = newPass
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            this.options.UpdateClaims(scope, userId);
            
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
            
            var handler = new ChangePasswordCommandHandler(
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
            
            this.options.UpdateClaims(scope, null);
            
            await TestUtilities.Handle_InvalidCommand<AuthLoginQuery, TokenResponse, AuthenticationException>(
                loginHandler, 
                new AuthLoginQuery(user.Login, user.Email, user.PhoneNumber, StringExtension.GenerateRandomPassword(12, 15), false), 
                errorMessage);
        }
    }
}