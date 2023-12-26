using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Tariff.Handlers;
using CIYW.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Tariff;
using CYIW.Mapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Tariff
{
    [TestClass]
    public class TariffQueryHandlerTest
    {
        private readonly IMapper mapper;
        private readonly Mock<IReadGenericRepository<Domain.Models.Tariff.Tariff>> tariffReadRepositoryMock;
        private readonly Mock<IEntityValidator> entityValidatorMock;

        private readonly TariffQueryHandler handler;

        public TariffQueryHandlerTest()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            
            this.mapper = configuration.CreateMapper();
            this.tariffReadRepositoryMock = new Mock<IReadGenericRepository<Domain.Models.Tariff.Tariff>>();
            this.entityValidatorMock = new Mock<IEntityValidator>();
            
            this.handler = new TariffQueryHandler(
                this.mapper,
                this.tariffReadRepositoryMock.Object,
                this.entityValidatorMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsTariffResponse()
        {
            Domain.Models.Tariff.Tariff tariff = InitializationProvider.GetFreeTariff();
            TariffResponse expected = this.mapper.Map<Domain.Models.Tariff.Tariff, TariffResponse>(tariff);
            this.tariffReadRepositoryMock
                .Setup(t => 
                    t.GetByIdAsync(It.IsAny<Guid>(), 
                        CancellationToken.None))
                .ReturnsAsync(tariff);

            TariffQuery query = new TariffQuery(tariff.Id);
            
            var result = await this.handler.Handle(query, CancellationToken.None);
            Assert.IsNotNull(result);
            result.Should().BeEquivalentTo(expected);
        }
        
        [TestMethod]
        public async Task Handle_InvalidQuery_ReturnsNull()
        {
            Guid? tariffId = Guid.NewGuid();
            TariffQuery query = new TariffQuery(tariffId.Value);

            string errorMessage = String.Format(ErrorMessages.EntityWithIdNotFound,
                typeof(Domain.Models.Tariff.Tariff).Name, tariffId);
            this.entityValidatorMock.Setup(
                    e => e.ValidateExist(It.IsAny<Domain.Models.Tariff.Tariff>(), tariffId))
                .Throws(new LoggerException(errorMessage, 404));
            
            await TestUtilities.Handle_InvalidCommand<TariffQuery, TariffResponse>(this.handler, query, errorMessage);
        }
    }
}

