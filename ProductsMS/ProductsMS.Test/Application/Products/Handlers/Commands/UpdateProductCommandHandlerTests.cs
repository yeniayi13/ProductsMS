using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Application.Products.Handlers.Commands;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;
using ProductsMS.Core.RabbitMQ;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using Xunit;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Test.Application.Products.Handlers.Commands
{
    
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IEventBus<UpdateProductDto>> _eventBusMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _eventBusMock = new Mock<IEventBus<UpdateProductDto>>();

            _handler = new UpdateProductCommandHandler(
                _productRepositoryMock.Object,
                _eventBusMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProductAndPublishEvent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var updateCommand = new UpdateProductCommand(productId, new UpdateProductDto
            {
                ProductName = "Updated Name",
                ProductPrice = 100,
                ProductStock = 50,
                ProductAvilability = "Disponible" // Enum corregido
            }, userId);

            var oldProduct = new ProductEntity(
                ProductId.Create(productId), // Corregido: Eliminado el segundo parámetro en Create()
                ProductName.Create("Old Name"),
                ProductImage.FromBase64("c29tZSBkYXRhIGVuIGJhc2U2NA=="),
                ProductPrice.Create(80),
                ProductDescription.Create("Old Description"),
                ProductAvilability.Disponible,
                ProductStock.Create(20),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(userId)
            );

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync(oldProduct);

            _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<ProductEntity>()))
                .ReturnsAsync((ProductEntity updatedProduct) => updatedProduct ?? new ProductEntity());

            _eventBusMock.Setup(bus => bus.PublishMessageAsync(It.IsAny<UpdateProductDto>(), "productQueue", "PRODUCT_UPDATED"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            Assert.Equal(productId, result);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ProductEntity>()), Times.Once);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(It.IsAny<UpdateProductDto>(), "productQueue", "PRODUCT_UPDATED"), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var updateCommand = new UpdateProductCommand(productId, new UpdateProductDto
            {
                ProductName = "Valid Name",
                ProductPrice = 50,
                ProductStock = 10,
                ProductAvilability = "Disponible" // Valor válido
            }, userId);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync((ProductEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _handler.Handle(updateCommand, CancellationToken.None));
            Assert.Equal("Product not found", exception.Message);

            _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ProductEntity>()), Times.Never);
            _eventBusMock.Verify(bus => bus.PublishMessageAsync(It.IsAny<UpdateProductDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Contains("Request cannot be null.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductAvailabilityIsInvalid()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var updateCommand = new UpdateProductCommand(productId,  new UpdateProductDto
            {
                ProductName = "Valid Name",
                ProductPrice = 50,
                ProductStock = 10,
                ProductAvilability = "No valido" // Valor inválido
            },userId);

            var oldProduct = new ProductEntity();
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ProductId>(), It.IsAny<ProductUserId>()))
                .ReturnsAsync(oldProduct);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(updateCommand, CancellationToken.None));
            Assert.Equal("Invalid ProductAvailability value", exception.Message);
        }
    }
}
