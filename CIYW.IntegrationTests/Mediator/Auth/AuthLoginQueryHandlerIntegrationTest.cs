using CIYW.Auth.Tokens;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Handlers;
using CIYW.Mediator.Mediator.Auth.Queries;
using CIYW.Models.Responses.Auth;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Auth;

[TestFixture]
public class AuthLoginQueryHandlerIntegrationTest() : CommonIntegrationTestSetup(null)
{
    private static IEnumerable<TestCaseData> CreateAuthLoginTestCases()
    {   
        yield return new TestCaseData("anime.kit", null, null, "zcbm13579", LoginProvider.CIYWLogin, null);
        yield return new TestCaseData(null, "animekit@mail.com", null, "zcbm13579", LoginProvider.CIYWEmail, null);
        yield return new TestCaseData(null, null, "22334433221", "zcbm13579", LoginProvider.CIYWPhone, null);
        yield return new TestCaseData("anime.kit1", null, null, "zcbm13579", LoginProvider.CIYWLogin, ErrorMessages.UserNotFound);
        yield return new TestCaseData(null, "animekit1@mail.com", null, "zcbm13579", LoginProvider.CIYWEmail, ErrorMessages.UserNotFound);
        yield return new TestCaseData(null, null, "22334433222", "zcbm13579", LoginProvider.CIYWPhone, ErrorMessages.UserNotFound);
        yield return new TestCaseData("anime.kit", null, null, "zcbm135791", LoginProvider.CIYWLogin, ErrorMessages.WrongAuth);
        yield return new TestCaseData(null, "animekit@mail.com", null, "zcbm135791", LoginProvider.CIYWEmail, ErrorMessages.WrongAuth);
        yield return new TestCaseData(null, null, "22334433221", "zcbm135791", LoginProvider.CIYWPhone, ErrorMessages.WrongAuth);
    }
    
    [Test, TestCaseSource(nameof(CreateAuthLoginTestCases))]
    public async Task Handle_ValidUserAuthLoginWithLogin_ReturnsTokenResponse(string login, string email, string phone, string password, string provider, string errorMessage )
    {
        // Arrange
        AuthLoginQuery query = MockCommandQueryHelper.CreateAuthLoginQuery();
        query.Login = login;
        query.Email = email;
        query.Phone = phone;
        query.Password = password;
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new AuthLoginQueryHandler(
                scope.ServiceProvider.GetRequiredService<IAuthRepository>(),
                scope.ServiceProvider.GetRequiredService<TokenGenerator>(),
                scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>()
            );

            var temp = dbContext.Invoices.Where(r => r.UserId == InitConst.MockUserId).ToList();

            // Act
            if (errorMessage.NotNullOrEmpty())
            {
                await TestUtilities.Handle_InvalidCommand<AuthLoginQuery, TokenResponse, AuthenticationException>(handler, query, errorMessage);
            }
            else
            {
                TokenResponse result = await handler.Handle(query, CancellationToken.None);

                // Assert
                result.Should().NotBeNull();
                dbContext.UserTokens
                    .Count(ut => ut.UserId == InitConst.MockAuthUserId && ut.LoginProvider == LoginProvider.CIYWLogin).Should()
                    .Be(1);
            }
        }
    }
}