using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductosMs.Controllers;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using Xunit;
using ProductosMs.Application.Products.Queries;

namespace ProductsMS.Test.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockLogger.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task CreatedProduct_ShouldReturnOk_WhenProductIsCreated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto { ProductName = "Smartphone" };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid()); // Simula creación exitosa

            // Act
            var result = await _controller.CreatedProduct(createProductDto, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreatedProduct_ShouldReturnBadRequest_WhenInvalidDataProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto(); // Datos incompletos

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NullAttributeException("Product data is required"));

            // Act
            var result = await _controller.CreatedProduct(createProductDto, userId);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Product data is required", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto>
            {
                new GetProductDto { ProductId = Guid.NewGuid(), ProductName = "Laptop" }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetAllProducts(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnNotFound_WhenNoProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GetProductDto>());

            // Act
            var result = await _controller.GetAllProducts(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("No products found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto>
            {
                new GetProductDto { ProductId = Guid.NewGuid(), ProductName = "Monitor", ProductAvilability = "Disponible" }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetAvailableProducts(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldReturnBadRequest_WhenUserIdIsEmpty()
        {
            // Act
            var result = await _controller.GetAvailableProducts(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("El ID del usuario es requerido.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var expectedProduct = new GetProductDto { ProductId = productId, ProductName = "Headphones" };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _controller.GetProduct(productId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ProductNotFoundException("Product not found"));

            // Act
            var result = await _controller.GetProduct(productId, userId);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, notFoundResult.StatusCode);
            Assert.Equal("An error occurred while trying to search an Product", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { ProductName = "Updated Laptop" };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productId);

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}

