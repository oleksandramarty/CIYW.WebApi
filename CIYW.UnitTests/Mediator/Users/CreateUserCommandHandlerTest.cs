using System.Linq.Expressions;
using AutoMapper;
using CIYW.Const.Const;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Auth.Handlers;
using CIYW.Mediator.Auth.Queries;
using CIYW.TestHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Users
{
    [TestClass]
    public class CreateUserCommandHandlerTest
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IEntityValidator> entityValidatorMock;
        private readonly Mock<IAuthRepository> authRepositoryMock;
        private readonly Mock<UserManager<User>> userManagerMock;

        private readonly CreateUserCommandHandler handler;

        public CreateUserCommandHandlerTest()
        {
            this.mapperMock = new Mock<IMapper>();
            
            this.mapperMock.Setup(m => m.Map<CreateUserCommand, User>(It.IsAny<CreateUserCommand>()))
                .Returns(new User());

            this.entityValidatorMock = new Mock<IEntityValidator>();
            this.authRepositoryMock = new Mock<IAuthRepository>();
            this.userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object
            );
            
            this.userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            this.userManagerMock
                .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            this.handler = new CreateUserCommandHandler(
                this.mapperMock.Object,
                this.entityValidatorMock.Object,
                this.authRepositoryMock.Object,
                this.userManagerMock.Object
            );

        }

        [TestMethod]
        public async Task Handle_ValidCommand_CreatesUser()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            
            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);
            
            List<IdentityUserLogin<Guid>> expectedLogins = result.CreateUserLogins();
            
            // Assert
            Assert.IsNotNull(result);
            this.userManagerMock.Verify(um => um.CreateAsync(It.Is<User>(u => u.Id == result.Id), command.Password), Times.Once);
            this.userManagerMock.Verify(um => um.AddToRoleAsync(It.Is<User>(u => u.Id == result.Id), RoleProvider.User), Times.Once);
            this.authRepositoryMock.Verify(repo => repo.UpdateUserLoginsAsync(
                It.Is<Guid>(u => u == result.Id),
                It.Is<List<IdentityUserLogin<Guid>>>(logins => 
                    logins.Count == expectedLogins.Count &&
                    logins.All(login => expectedLogins.Any(expectedLogin =>
                        login.LoginProvider == expectedLogin.LoginProvider &&
                        login.ProviderKey == expectedLogin.ProviderKey &&
                        login.ProviderDisplayName == expectedLogin.ProviderDisplayName
                    ))
                ),
                CancellationToken.None), Times.Once);
        }
        
        [TestMethod]
        public async Task Handle_InvalidCommand_EmailException()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            command.ConfirmEmail = "123";
            
            // Act
            await TestUtilities.Handle_InvalidCommand<CreateUserCommand, User>(this.handler, command, ErrorMessages.EmailsDoesntMatch);
        }
        
        [TestMethod]
        public async Task Handle_InvalidCommand_PasswordException()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            command.ConfirmPassword = "123";
            
            // Act
            await TestUtilities.Handle_InvalidCommand<CreateUserCommand, User>(this.handler, command, ErrorMessages.PasswordsDoesntMatch);
        }
        
        [TestMethod]
        public async Task Handle_InvalidCommand_AgreeException()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            command.IsAgree = false;
            
            // Act
            await TestUtilities.Handle_InvalidCommand<CreateUserCommand, User>(this.handler, command, ErrorMessages.AgreeBeforeSignIn);
        }
        
        [TestMethod]
        public async Task Handle_InvalidCommand_EmailExists()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            string errorMessage = String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Email);
            this.AddEntityValidatorThrow(errorMessage);
            
            // Act
            await TestUtilities.Handle_InvalidCommand<CreateUserCommand, User>(this.handler, command, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Email));
        }
        
        [TestMethod]
        public async Task Handle_InvalidCommand_PhoneExists()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            string errorMessage = String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Phone);
            this.AddEntityValidatorThrow(errorMessage);
            
            // Act
            await TestUtilities.Handle_InvalidCommand<CreateUserCommand, User>(this.handler, command, errorMessage);
        }
        
        [TestMethod]
        public async Task Handle_InvalidCommand_LoginExists()
        {
            // Arrange
            var command = MockCommandQueryHelper.CreateCreateUserCommand();
            string errorMessage = String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Login);
            this.AddEntityValidatorThrow(errorMessage);
            
            // Act
            await TestUtilities.Handle_InvalidCommand<CreateUserCommand, User>(this.handler, command, errorMessage);
        }

        private void AddEntityValidatorThrow(string errorMessage)
        {
            this.entityValidatorMock.Setup(ev => ev.ValidateExistParamAsync<User>(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<string>(),
                    CancellationToken.None))
                .Throws(new LoggerException(errorMessage, 409));
        }


    }
}