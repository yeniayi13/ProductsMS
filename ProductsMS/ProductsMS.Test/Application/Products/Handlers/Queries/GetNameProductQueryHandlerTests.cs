using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using ProductsMS.Application.Products.Handlers.Queries;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMS.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using Xunit;


namespace ProductsMS.Test.Application.Products.Handlers.Queries
{
  
    public class GetNameProductQueryHandlerTests
    {
        private readonly Mock<IProductRepositoryMongo> _productRepositoryMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetNameProductQueryHandler _handler;

        public GetNameProductQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepositoryMongo>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetNameProductQueryHandler(
                _productRepositoryMock.Object,
                _dbContextMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetNameProductQuery("Existing Product", userId);

            var product = new ProductEntity();
            var productDto = new GetProductDto();

            _productRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<ProductName>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(mapper => mapper.Map<GetProductDto>(product))
                .Returns(productDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDto, result);
            _productRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<ProductName>(), It.IsAny<ProductUserId>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<GetProductDto>(product), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetNameProductQuery("Nonexistent Product", userId);

            _productRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<ProductName>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync((ProductEntity)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _productRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<ProductName>(), It.IsAny<ProductUserId>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMapperIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => new GetAllProductQueryHandler(_productRepositoryMock.Object, null)));
            Assert.Contains("mapper", exception.Message);
        }
    }
}
