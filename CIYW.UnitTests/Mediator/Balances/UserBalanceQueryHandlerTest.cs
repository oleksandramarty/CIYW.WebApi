using System.Linq.Expressions;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Balances.Handlers;
using CIYW.Mediator.Mediator.Balances.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Balances
{
    [TestClass]
    public class UserBalanceQueryHandlerTest
    {
        private readonly Mock<IReadGenericRepository<UserBalance>> balanceReadRepositoryMock;
        private readonly Mock<ICurrentUserProvider> currentUserProviderMock;

        private readonly UserBalanceQueryHandler handler;
        private readonly UserBalance balance;

        public UserBalanceQueryHandlerTest()
        {
            this.balance = InitializationProvider.GetUserBalance(InitConst.MockUserId, InitConst.CurrencyUsdId, 1000.0m);
            
            this.balanceReadRepositoryMock = new Mock<IReadGenericRepository<UserBalance>>();
            this.currentUserProviderMock = new Mock<ICurrentUserProvider>();
            
            this.currentUserProviderMock.Setup(r => r.GetUserIdAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(InitConst.MockUserId);

            this.balanceReadRepositoryMock.Setup(r => r.GetByPropertyAsync(
                It.IsAny<Expression<Func<UserBalance, bool>>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(this.balance);
            
            this.handler = new UserBalanceQueryHandler(
                this.balanceReadRepositoryMock.Object,
                this.currentUserProviderMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsBalanceAmount()
        {
            UserBalanceQuery query = new UserBalanceQuery();
            
            var result = await this.handler.Handle(query, CancellationToken.None);
            Assert.AreEqual(this.balance.Amount, result);
        }
    }
}