using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Core.Repository;

namespace ProductsMS.Test.Application.Products.Handlers.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using ProductsMS.Application.Products.Handlers.Queries;
    using ProductsMS.Application.Products.Queries;
    using ProductsMS.Common.Dtos.Product.Response;
    using ProductsMs.Core.Repository;
    using ProductsMs.Domain.Entities.Category.ValueObject;
    using ProductsMS.Domain.Entities.Products.ValueObjects;
    using ProductsMs.Domain.Entities.Products;
    using Xunit;

    public class GetAvailableProductsQueryHandlerTests
    {
        private readonly Mock<IProductRepositoryMongo> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAvailableProductsQueryHandler _handler;

        public GetAvailableProductsQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepositoryMongo>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAvailableProductsQueryHandler(
                _productRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAvailableProducts_WhenProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetAvailableProductsQuery(userId, null, 10, 200);

            var productList = new List<ProductEntity> { new ProductEntity(), new ProductEntity() };
            var productDtoList = new List<GetProductDto> { new GetProductDto(), new GetProductDto() };

            _productRepositoryMock.Setup(repo => repo.GetAvailableProductsAsync(It.IsAny<ProductUserId>(), It.IsAny<CategoryId>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ReturnsAsync(productList);

            _mapperMock.Setup(mapper => mapper.Map<List<GetProductDto>>(productList))
                .Returns(productDtoList);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDtoList.Count, result.Count);
            _productRepositoryMock.Verify(repo => repo.GetAvailableProductsAsync(It.IsAny<ProductUserId>(), It.IsAny<CategoryId>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<GetProductDto>>(productList), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetAvailableProductsQuery(userId, null, 10, 200);

            _productRepositoryMock.Setup(repo => repo.GetAvailableProductsAsync(It.IsAny<ProductUserId>(), It.IsAny<CategoryId>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ReturnsAsync(new List<ProductEntity>());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _productRepositoryMock.Verify(repo => repo.GetAvailableProductsAsync(It.IsAny<ProductUserId>(), It.IsAny<CategoryId>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetAvailableProductsQuery(userId, null, 10, 200);

            _productRepositoryMock.Setup(repo => repo.GetAvailableProductsAsync(It.IsAny<ProductUserId>(), It.IsAny<CategoryId>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // El método maneja excepciones devolviendo una lista vacía
            _productRepositoryMock.Verify(repo => repo.GetAvailableProductsAsync(It.IsAny<ProductUserId>(), It.IsAny<CategoryId>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once);
        }
    }
}
