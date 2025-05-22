using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentValidation;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Application.Products.Handlers.Commands;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Enum;
using ProductsMS.Core.RabbitMQ;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Products;
using AutoMapper;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Core.Service.User;

namespace ProductsMS.Test.Application.Products.Handlers.Commands
{

    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IEventBus<GetProductDto>> _eventBusMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _eventBusMock = new Mock<IEventBus<GetProductDto>>();
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateProductCommandHandler(
                _mapperMock.Object,
                _userServiceMock.Object,
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _eventBusMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateProductAndPublishEvent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productDto = new CreateProductDto
            {
                ProductId = productId,
                ProductName = "New Product",
                ProductImage = "c29tZSBkYXRhIGVuIGJhc2U2NA==",
                ProductPrice = 150,
                ProductDescription = "A new amazing product",
                ProductAvilability = "Disponible",
                ProductStock = 50,
                CategoryId = categoryId,
                ProductUserId = userId
            };
            var getProductDto = new GetProductDto
            {
                ProductId = productId,
                ProductName = "New Product",
                ProductImage = "c29tZSBkYXRhIGVuIGJhc2U2NA==",
                ProductPrice = 150,
                ProductDescription = "A new amazing product",
                ProductAvilability = "Disponible",
                ProductStock = 50,
                CategoryId = categoryId,
                ProductUserId = userId
            };
            var createCommand = new CreateProductCommand(productDto, userId);

            var category = new CategoryEntity(CategoryId.Create(categoryId), CategoryName.Create("Hogar"));
            var user = new GetUser
            {
                UserId = userId,
                UserName = "Test User"
            };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<CategoryId>()))
                .ReturnsAsync(category);

            _userServiceMock.Setup(service => service.AuctioneerExists(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            _productRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<ProductEntity>()))
                .Returns(Task.CompletedTask);

            _eventBusMock.Setup(bus =>
                    bus.PublishMessageAsync(It.IsAny<GetProductDto>(), "productQueue", "PRODUCT_CREATED"))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(mapper => mapper.Map<GetProductDto>(It.IsAny<ProductEntity>()))
                .Returns(getProductDto);

            // Act
            var result = await _handler.Handle(createCommand, CancellationToken.None);

            // Assert
            Assert.Equal(productId, result);
            _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProductEntity>()), Times.Once);
            _eventBusMock.Verify(
                bus => bus.PublishMessageAsync(It.IsAny<GetProductDto>(), "productQueue", "PRODUCT_CREATED"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var createCommand = new CreateProductCommand(new CreateProductDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = "", // Nombre inválido
                ProductPrice = -1, // Precio inválido
                ProductStock = 0, // Stock inválido
                CategoryId = Guid.NewGuid(),
                ProductUserId = Guid.NewGuid()
            }, Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _handler.Handle(createCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var createCommand = new CreateProductCommand(new CreateProductDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Valid Product",
                ProductPrice = 100,
                ProductStock = 10,
                CategoryId = Guid.NewGuid(),
                ProductUserId = Guid.NewGuid(),
                ProductAvilability = "Disponible"
            }, Guid.NewGuid());

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<CategoryId>()))
                .ReturnsAsync((CategoryEntity)null);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(createCommand, CancellationToken.None));
            Assert.Contains("The specified category does not exist.",
                exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var createCommand = new CreateProductCommand(new CreateProductDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Valid Product",
                ProductPrice = 100,
                ProductStock = 10,
                CategoryId = Guid.NewGuid(),
                ProductUserId = Guid.NewGuid(),
                ProductAvilability = "Disponible"
            }, Guid.NewGuid());

            _userServiceMock.Setup(service => service.AuctioneerExists(It.IsAny<Guid>()))
                .ReturnsAsync((GetUser)null);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(createCommand, CancellationToken.None));
            Assert.Contains("The specified category does not exist.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRequestIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(null, CancellationToken.None));
            Assert.Contains("Object reference not set to an instance",
                exception.Message);
        }
    }
}
