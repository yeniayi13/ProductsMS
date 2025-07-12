using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using ProductosMs.Application.Products.Queries;
using ProductsMS.Application.Products.Handlers.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMS.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using Xunit;

namespace ProductsMS.Test.Application.Products.Handlers.Queries
{
    
    public class GetProductQueryHandlerTests
    {
        private readonly Mock<IProductRepositoryMongo> _productRepositoryMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetProductQueryHandler _handler;

        public GetProductQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepositoryMongo>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetProductQueryHandler(
                _productRepositoryMock.Object,
                _dbContextMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new GetProductQuery(productId, userId);

            var product = new ProductEntity();
            var productDto = new GetProductDto();

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(mapper => mapper.Map<GetProductDto>(product))
                .Returns(productDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDto, result);
            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<GetProductDto>(product), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductIdIsEmpty()
        {
            // Arrange
            var request = new GetProductQuery(Guid.Empty, Guid.NewGuid());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullAttributeException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Product id is required", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new GetProductQuery(productId, userId);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync((ProductEntity)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMapperIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                Task.Run(() => new GetAllProductQueryHandler(_productRepositoryMock.Object, null)));
            Assert.Contains("mapper", exception.Message);
        }
    }
}
