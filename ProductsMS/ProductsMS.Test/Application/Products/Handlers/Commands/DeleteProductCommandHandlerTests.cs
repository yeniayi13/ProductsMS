using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Moq;
using ProductsMS.Application.Products.Commands;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Handlers.Commands;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using Xunit;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Core.Repository;
using ProductsMS.Common.Exceptions;
using ProductsMS.Core.Service.Auction;


namespace ProductsMS.Test.Application.Products.Handlers.Commands
{

    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IEventBus<GetProductDto>> _eventBusMock;
        private readonly Mock<IProductRepositoryMongo> _productRepositoryMongoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAuctionService> _auctionServiceMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _eventBusMock = new Mock<IEventBus<GetProductDto>>();
            _mapperMock = new Mock<IMapper>();
            _productRepositoryMongoMock = new Mock<IProductRepositoryMongo>();
            _auctionServiceMock = new Mock<IAuctionService>();

            _handler = new DeleteProductCommandHandler(
                _productRepositoryMongoMock.Object,
                _productRepositoryMock.Object,
                _eventBusMock.Object,
                _mapperMock.Object,_auctionServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProductAndPublishEvent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var deleteCommand = new DeleteProductCommand(productId, userId);

            var product = new ProductEntity(); // Simulación del producto recuperado
            _productRepositoryMongoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync(product);

            _productRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<ProductId>()))
                .Returns(Task.CompletedTask);

            var productDto = new GetProductDto();
            _mapperMock.Setup(mapper => mapper.Map<GetProductDto>(product))
                .Returns(productDto);

            _eventBusMock.Setup(bus => bus.PublishMessageAsync(productDto, "productQueue", "PRODUCT_DELETED"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(deleteCommand, CancellationToken.None);

            // Assert
            Assert.Equal(productId, result);
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<ProductId>()), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(productDto, "productQueue", "PRODUCT_DELETED"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var deleteCommand = new DeleteProductCommand(productId, userId);

            _productRepositoryMongoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync((ProductEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _handler.Handle(deleteCommand, CancellationToken.None));
            Assert.Equal("Product not found.", exception.Message);

            _productRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<ProductId>()), Times.Never);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(It.IsAny<GetProductDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Contains("Request cannot be null.", exception.Message);
        }
    
    }
}
