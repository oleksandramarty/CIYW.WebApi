using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediatr.Currency.Handlers;
using CIYW.Mediator.Mediatr.Currency.Requests;
using CIYW.Models.Responses.Currency;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Currency
{
    [TestClass]
    public class CurrencyQueryHandlerTest
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IReadGenericRepository<Domain.Models.Currency.Currency>> currencyReadRepositoryMock;
        private readonly Mock<IEntityValidator> entityValidatorMock;

        private readonly CurrencyQueryHandler handler;
        private readonly Domain.Models.Currency.Currency currency;

        private readonly CurrencyResponse expected;

        public CurrencyQueryHandlerTest()
        {
            this.mapperMock = new Mock<IMapper>();
            this.currency = InitializationProvider.GetUSDCurrency();
            this.expected = new CurrencyResponse
            {
                Id = currency.Id,
                Name = currency.Name,
                Symbol = currency.Symbol,
                IsoCode = currency.IsoCode
            };
            this.mapperMock.Setup(m => m.Map<Domain.Models.Currency.Currency, CurrencyResponse>(It.IsAny<Domain.Models.Currency.Currency>()))
                .Returns(expected);
            
            this.currencyReadRepositoryMock = new Mock<IReadGenericRepository<Domain.Models.Currency.Currency>>();
            this.entityValidatorMock = new Mock<IEntityValidator>();
            
            this.handler = new CurrencyQueryHandler(
                this.mapperMock.Object,
                this.currencyReadRepositoryMock.Object,
                this.entityValidatorMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsCurrencyResponse()
        {
            CurrencyQuery query = new CurrencyQuery(this.currency.Id);
            
            var result = await this.handler.Handle(query, CancellationToken.None);
            Assert.IsNotNull(result);
            result.Should().BeEquivalentTo(expected);
        }
        
        [TestMethod]
        public async Task Handle_InvalidQuery_NotFoundException()
        {
            Guid? currencyId = Guid.NewGuid();
            CurrencyQuery query = new CurrencyQuery(currencyId.Value);

            string errorMessage = String.Format(ErrorMessages.EntityWithIdNotFound,
                typeof(Domain.Models.Currency.Currency).Name, currencyId);
            this.entityValidatorMock.Setup(
                    e => e.ValidateExist(It.IsAny<Domain.Models.Currency.Currency>(), currencyId))
                .Throws(new LoggerException(errorMessage, 404));
            
            await TestUtilities.Handle_InvalidCommand<CurrencyQuery, CurrencyResponse>(this.handler, query, errorMessage);
        }
    }
}
