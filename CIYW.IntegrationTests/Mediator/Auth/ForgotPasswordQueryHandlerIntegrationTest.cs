using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Common;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Auth.Handlers;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Auth;

[TestFixture]
public class ForgotPasswordQueryHandlerIntegrationTest() : CommonIntegrationTestSetup(null)
{
    private static IEnumerable<TestCaseData> CreateValidForgotPasswordQueryTestCases()
    {   
        yield return new TestCaseData("john.doe", "myemail@mail.com", "12124567890", InitConst.MockUserId);
        yield return new TestCaseData("anime.kit", "animekit@mail.com", "22334433221", InitConst.MockAuthUserId);
        yield return new TestCaseData("admin.test", "admintest@mail.com", "44332255332", InitConst.MockAdminUserId);

    }
    
    [Test, TestCaseSource(nameof(CreateValidForgotPasswordQueryTestCases))]
    public async Task Handle_ValidForgotPasswordQuery_GeneratesUrlToken(
        string login,
        string email,
        string phone,
        Guid expectedUserId)
    {
        // Arrange
        ForgotPasswordQuery query = new ForgotPasswordQuery
        {
            Login = login,
            Email = email,
            Phone = phone
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            await this.RemoveRestorePasswordDataAsync(CancellationToken.None);
            
            var handler = new ForgotPasswordQueryHandler(
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<RestorePassword>>(),
                scope.ServiceProvider.GetRequiredService<IConfiguration>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            dbContext.RestorePasswords
                .Any(ut => ut.UserId == expectedUserId).Should()
                .Be(true);
        }
    }
    
    private static IEnumerable<TestCaseData> CreateInvalidForgotPasswordQueryTestCases()
    {   
        yield return new TestCaseData("john1.doe", "myemail@mail.com", "12124567890", null, String.Format(ErrorMessages.EntityWithIdNotFound, nameof(User), null));
        yield return new TestCaseData("john1.doe", "myemail1@mail.com", "12124567890", null, String.Format(ErrorMessages.EntityWithIdNotFound, nameof(User), null));
        yield return new TestCaseData("john1.doe", "myemail@mail.com", "12324567890", null, String.Format(ErrorMessages.EntityWithIdNotFound, nameof(User), null));
        yield return new TestCaseData("john.doe", "myemail@mail.com", "12124567890", DateTime.UtcNow, ErrorMessages.TryRestoreALittleLater);
    }
    
    [Test, TestCaseSource(nameof(CreateInvalidForgotPasswordQueryTestCases))]
    public async Task Handle_InvalidForgotPasswordQuery_ReturnsException(
        string login,
        string email,
        string phone,
        DateTime? restored,
        string expectedError)
    {
        // Arrange
        ForgotPasswordQuery query = new ForgotPasswordQuery
        {
            Login = login,
            Email = email,
            Phone = phone
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            if (restored.HasValue)
            {
                User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == InitConst.MockUserId,
                    CancellationToken.None);
                user.Restored = restored;
                dbContext.Users.Update(user);
            }
            dbContext.RestorePasswords.RemoveRange(dbContext.RestorePasswords);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            
            var handler = new ForgotPasswordQueryHandler(
                scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<RestorePassword>>(),
                scope.ServiceProvider.GetRequiredService<IConfiguration>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<ForgotPasswordQuery, LoggerException>(
                handler, 
                query, 
                expectedError);
        }
    }
}