using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Tariff.Handlers;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Tariff;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CIYW.UnitTests.Mediator.Tariff
{
    [TestClass]
    public class TariffQueryHandlerTest
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IReadGenericRepository<Domain.Models.Tariff.Tariff>> tariffReadRepositoryMock;
        private readonly Mock<IEntityValidator> entityValidatorMock;
        private readonly Mock<ICurrentUserProvider> currentUserProviderMock;

        private readonly TariffQueryHandler handler;

        public TariffQueryHandlerTest()
        {
            this.mapperMock = new Mock<IMapper>();
            Domain.Models.Tariff.Tariff tariff = InitializationProvider.GetFreeTariff();
            this.mapperMock.Setup(m => m.Map<Domain.Models.Tariff.Tariff, TariffResponse>(It.IsAny<Domain.Models.Tariff.Tariff>()))
                .Returns(new TariffResponse
                {
                    Id = tariff.Id,
                    Name = tariff.Name,
                    Created = tariff.Created,
                    Description = tariff.Description
                });
            
            this.tariffReadRepositoryMock = new Mock<IReadGenericRepository<Domain.Models.Tariff.Tariff>>();
            this.entityValidatorMock = new Mock<IEntityValidator>();
            this.currentUserProviderMock = new Mock<ICurrentUserProvider>();
            
            this.handler = new TariffQueryHandler(
                this.mapperMock.Object,
                this.tariffReadRepositoryMock.Object,
                this.entityValidatorMock.Object,
                this.currentUserProviderMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsTariffResponse()
        {
            Domain.Models.Tariff.Tariff tariff = InitializationProvider.GetFreeTariff();
            TariffResponse expected = new TariffResponse
            {
                Id = tariff.Id,
                Name = tariff.Name,
                Created = tariff.Created,
                Description = tariff.Description
            };
            this.tariffReadRepositoryMock
                .Setup(t => 
                    t.GetByIdAsync(It.IsAny<Guid>(), 
                        CancellationToken.None))
                .ReturnsAsync(tariff);

            TariffQuery query = new TariffQuery(tariff.Id);
            
            var result = await this.handler.Handle(query, CancellationToken.None);
            Assert.IsNotNull(result);
            result.MappedEntity.Should().BeEquivalentTo(expected, options => options.Excluding(o => o.Created));
        }
        
        [TestMethod]
        public async Task Handle_InvalidQuery_NotFoundException()
        {
            Guid? tariffId = Guid.NewGuid();
            TariffQuery query = new TariffQuery(tariffId.Value);

            string errorMessage = String.Format(ErrorMessages.EntityWithIdNotFound,
                typeof(Domain.Models.Tariff.Tariff).Name, tariffId);
            this.entityValidatorMock.Setup(
                    e => e.ValidateExist(It.IsAny<Domain.Models.Tariff.Tariff>(), tariffId))
                .Throws(new LoggerException(errorMessage, 404));
            
            await TestUtilities.Handle_InvalidCommand<TariffQuery, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>, LoggerException>(this.handler, query, errorMessage);
        }
    }
}

