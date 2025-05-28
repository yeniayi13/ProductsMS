using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using ProductsMS.Application.Products.Handlers.Queries;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;
using ProductsMs.Core.Repository;
using ProductsMS.Core.Repository;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using Xunit;

namespace ProductsMS.Test.Application.Products.Handlers.Queries
{
    
    public class GetAllProductQueryHandlerTests
    {
        private readonly Mock<IProductRepositoryMongo> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllProductQueryHandler _handler;

        public GetAllProductQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepositoryMongo>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllProductQueryHandler(
                _productRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductList_WhenProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetAllProductQuery(userId);

            var productList = new List<ProductEntity> { new ProductEntity(), new ProductEntity() };
            var productDtoList = new List<GetProductDto> { new GetProductDto(), new GetProductDto() };

            _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<ProductUserId>()))
                .ReturnsAsync(productList);

            _mapperMock.Setup(mapper => mapper.Map<List<GetProductDto>>(productList))
                .Returns(productDtoList);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDtoList.Count, result.Count);
            _productRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<ProductUserId>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<GetProductDto>>(productList), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductsAreEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetAllProductQuery(userId);

            _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<ProductUserId>()))
                .ReturnsAsync((List<ProductEntity>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Products are empty", exception.Message);
        }

        //[Fact]

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
