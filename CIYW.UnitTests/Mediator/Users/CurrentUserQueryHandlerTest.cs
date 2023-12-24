﻿using System.Linq.Expressions;
using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Users.Handlers;
using CIYW.Mediator.Users.Requests;
using CYIW.Mapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Users
{
    [TestClass]
    public class CurrentUserQueryHandlerTests
    {
        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsCurrentUserResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            IMapper mapper = configuration.CreateMapper();
            
            var mediatorMock = new Mock<IMediator>();
            var userRepositoryMock = new Mock<IReadGenericRepository<User>>();
            var userRoleRepositoryMock = new Mock<IReadGenericRepository<IdentityUserRole<Guid>>>();
            var roleRepositoryMock = new Mock<IReadGenericRepository<Role>>();
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();

            var query = new CurrentUserQuery();
            User mockUser = MockHelper.GetMockUser();
            IList<IdentityUserRole<Guid>> mockRoles = MockHelper.GetMockRoles(mockUser.Id);
            Role mockRole = MockHelper.GetMockRole();
            currentUserProviderMock.Setup(r => r.GetUserIdAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUser.Id);
            userRepositoryMock.Setup(r =>
                    r.GetWithIncludeAsync(
                        It.IsAny<Func<User, bool>>(),
                        It.IsAny<CancellationToken>(),
                        null))
                .ReturnsAsync(mockUser);

            currentUserProviderMock.Setup(x => x.GetUserIdAsync(cancellationToken)).ReturnsAsync(userId);
            userRepositoryMock.Setup(x => x.GetWithIncludeAsync(It.IsAny<Func<User, bool>>(), cancellationToken, It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .ReturnsAsync(mockUser);
            userRoleRepositoryMock.Setup(x => x.GetListByPropertyAsync(It.IsAny<Expression<Func<IdentityUserRole<Guid>, bool>>>(), cancellationToken))
                .ReturnsAsync(mockRoles);
            roleRepositoryMock.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<Role, bool>>>(), cancellationToken))
                .ReturnsAsync(mockRole);

            var handler = new CurrentUserQueryHandler(
                mapper,
                mediatorMock.Object,
                userRepositoryMock.Object,
                userRoleRepositoryMock.Object,
                roleRepositoryMock.Object,
                currentUserProviderMock.Object
            );

            // Act
            var result = await handler.Handle(query, cancellationToken);

            // Assert
            Assert.IsNotNull(result);
        }


    }
}