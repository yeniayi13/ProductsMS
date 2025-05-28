using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MongoDB.Driver;
using Moq;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Enum;
using ProductsMs.Core.Database;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Infrastructure.Repositories;
using ProductsMS.Infrastructure.Repositories;
using Xunit;

namespace ProductsMS.Test.Infrastructure.Repositories
{
    public class ProductRepositoryMongoTests
    {
        private readonly Mock<IMongoCollection<ProductEntity>> _mockCollection = new();
        private readonly Mock<IMapper> _mockMapper = new();

        private readonly ProductRepositoryMongo _repository;

        public ProductRepositoryMongoTests()
        {
            _repository = new ProductRepositoryMongo( _mockCollection.Object, _mockMapper.Object);
        }

    
        // 🔍 PRUEBA: Obtener producto por ID - Éxito
        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var productId = ProductId.Create(Guid.NewGuid());
            var userId = ProductUserId.Create(Guid.NewGuid());

            var productDto = new GetProductDto { ProductId = productId.Value, ProductUserId = userId.Value };
            var productEntity = new ProductEntity(
                productId, // ID del producto
                ProductName.Create("Nombre del Producto"), // Nombre del producto
                ProductImage.FromBase64("https://mi-servidor.com/imagen_producto.jpg"), // Imagen del producto
                ProductPrice.Create(99.99m), // Precio del producto
                ProductDescription.Create("Descripción breve del producto"), // Descripción del producto
                ProductAvilability.Disponible, // Estado de disponibilidad
                ProductStock.Create(50), // Stock del producto
                CategoryId.Create(Guid.NewGuid()), // Categoría
                userId // Usuario propietario
            );

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new[] { productDto });

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    default))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<ProductEntity>(productDto)).Returns(productEntity);

            // Act
            var result = await _repository.GetByIdAsync(productId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId.Value, result.ProductId.Value);
            Assert.Equal(userId.Value, result.ProductUserId.Value);
        }


        [Fact]
        public async Task GetAvailableProductsAsync_ShouldReturnProducts_WhenExists()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());
            var categoryId = CategoryId.Create(Guid.NewGuid());

            var productDtos = new List<GetProductDto>
        {
            new() { ProductUserId = userId.Value, ProductAvilability = "Disponible", ProductPrice = 100, CategoryId = categoryId.Value }
        };

            var productEntities = new List<ProductEntity>
        {
            new(
                ProductId.Create(Guid.NewGuid()), // ID del producto
                ProductName.Create("Nombre del Producto"), // Nombre del producto
                ProductImage.FromBase64("https://mi-servidor.com/imagen_producto.jpg"), // Imagen del producto
                ProductPrice.Create(99.99m), // Precio del producto
                ProductDescription.Create("Descripción breve del producto"), // Descripción del producto
                ProductAvilability.Disponible, // Estado de disponibilidad
                ProductStock.Create(50), // Stock del producto
                CategoryId.Create(Guid.NewGuid()), // Categoría
                userId // Usuario propietario
            )
        };

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(productDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(productDtos)).Returns(productEntities);

            // Act
            var result = await _repository.GetAvailableProductsAsync(userId, categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Nombre del Producto", result[0].ProductName.Value);
        }

        [Fact]
        public async Task GetAvailableProductsAsync_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new List<GetProductDto>());

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(It.IsAny<List<GetProductDto>>()))
                .Returns(new List<ProductEntity>());

            // Act
            var result = await _repository.GetAvailableProductsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableProductsAsync_ShouldApplyFilters_Correctly()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());
            var categoryId = CategoryId.Create(Guid.NewGuid());
            decimal minPrice = 50;
            decimal maxPrice = 200;

            var productDtos = new List<GetProductDto>
        {
            new() { ProductUserId = userId.Value, ProductAvilability = "Disponible", ProductPrice = 100, CategoryId = categoryId.Value }
        };

            var productEntities = new List<ProductEntity>
        {
            new(
                ProductId.Create(Guid.NewGuid()), // ID del producto
                ProductName.Create("Nombre del Producto"), // Nombre del producto
                ProductImage.FromBase64("https://mi-servidor.com/imagen_producto.jpg"), // Imagen del producto
                ProductPrice.Create(99.99m), // Precio del producto
                ProductDescription.Create("Descripción breve del producto"), // Descripción del producto
                ProductAvilability.Disponible, // Estado de disponibilidad
                ProductStock.Create(50), // Stock del producto
                CategoryId.Create(Guid.NewGuid()), // Categoría
                userId // Usuario propietario
            )
        };

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(productDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(productDtos)).Returns(productEntities);

            // Act
            var result = await _repository.GetAvailableProductsAsync(userId, categoryId, minPrice, maxPrice);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Nombre del Producto", result[0].ProductName.Value);
        }

        [Fact]
        public async Task GetAvailableProductsAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _repository.GetAvailableProductsAsync(userId));
        }

        //  PRUEBA: Obtener producto por ID - No encontrado
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var productId = ProductId.Create(Guid.NewGuid());
            var userId = ProductUserId.Create(Guid.NewGuid());
            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<ProductEntity>>(), It.IsAny<FindOptions<ProductEntity, GetProductDto>>(), default))
                           .ReturnsAsync(mockCursor.Object);

            var result = await _repository.GetByIdAsync(productId, userId);

            Assert.Null(result);
        }

        //  PRUEBA: Obtener producto por nombre
        [Fact]
        public async Task GetByNameAsync_ShouldReturnProduct_WhenExists()
        {
            var name = ProductName.Create("Laptop");
            var userId = ProductUserId.Create(Guid.NewGuid());
            var productDto = new GetProductDto { ProductName = "Laptop", ProductUserId = userId.Value };

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new[] { productDto });

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<ProductEntity>>(), It.IsAny<FindOptions<ProductEntity, GetProductDto>>(), default))
                           .ReturnsAsync(mockCursor.Object);
            _mockMapper.Setup(m => m.Map<ProductEntity>(productDto)).Returns(new ProductEntity());

            var result = await _repository.GetByNameAsync(name, userId);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var name = ProductName.Create("NonExistentProduct");
            var userId = ProductUserId.Create(Guid.NewGuid());

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new List<GetProductDto>());

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<ProductEntity>>(),
                        It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetByNameAsync(name, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var name = ProductName.Create("Laptop");
            var userId = ProductUserId.Create(Guid.NewGuid());

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<ProductEntity>>(),
                        It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _repository.GetByNameAsync(name, userId));
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnFirstProduct_WhenMultipleProductsExist()
        {
            // Arrange
            var name = ProductName.Create("Laptop");
            var userId = ProductUserId.Create(Guid.NewGuid());
            var productDto1 = new GetProductDto { ProductName = "Laptop", ProductUserId = userId.Value };
            var productDto2 = new GetProductDto { ProductName = "Laptop", ProductUserId = userId.Value };

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new[] { productDto1, productDto2 });

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<ProductEntity>>(),
                        It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<ProductEntity>(productDto1)).Returns(new ProductEntity());

            // Act
            var result = await _repository.GetByNameAsync(name, userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldThrowException_WhenProductNameIsNull()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await _repository.GetByNameAsync(null, userId));
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenUserIdHasNoProducts()
        {
            // Arrange
            var name = ProductName.Create("Laptop");
            var userId = ProductUserId.Create(Guid.NewGuid());

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new List<GetProductDto>());

            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<ProductEntity>>(),
                        It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetByNameAsync(name, userId);

            // Assert
            Assert.Null(result);
        }



        //Pruebas de Get ALL
        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts_WhenExists()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            var productDtos = new List<GetProductDto>
        {
            new() { ProductUserId = userId.Value, ProductAvilability = "Disponible", ProductPrice = 100 }
        };

            var productEntities = new List<ProductEntity>
        {
            new(
                ProductId.Create(Guid.NewGuid()), // ID del producto
                ProductName.Create("Nombre del Producto"), // Nombre del producto
                ProductImage.FromBase64("c29tZSBkYXRhIGVuIGJhc2U2NA=="), // Imagen del producto
                ProductPrice.Create(99.99m), // Precio del producto
                ProductDescription.Create("Descripción breve del producto"), // Descripción del producto
                ProductAvilability.Disponible, // Estado de disponibilidad
                ProductStock.Create(50), // Stock del producto
                CategoryId.Create(Guid.NewGuid()), // Categoría
                userId // Usuario propietario
            )
        };

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(productDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(productDtos)).Returns(productEntities);

            // Act
            var result = await _repository.GetAllAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Nombre del Producto", result[0].ProductName.Value);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(new List<GetProductDto>());

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(It.IsAny<List<GetProductDto>>()))
                .Returns(new List<ProductEntity>());

            // Act
            var result = await _repository.GetAllAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _repository.GetAllAsync(userId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts_WithoutProjection()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            var productDtos = new List<GetProductDto>
        {
            new() { ProductUserId = userId.Value, ProductAvilability = "Disponible", ProductPrice = 200 }
        };

            var productEntities = new List<ProductEntity>
        {
            new(
                ProductId.Create(Guid.NewGuid()), // ID del producto
                ProductName.Create("Nombre del Producto"), // Nombre del producto
                ProductImage.FromBase64("c29tZSBkYXRhIGVuIGJhc2U2NA=="), // Imagen del producto
                ProductPrice.Create(99.99m), // Precio del producto
                ProductDescription.Create("Descripción breve del producto"), // Descripción del producto
                ProductAvilability.Disponible, // Estado de disponibilidad
                ProductStock.Create(50), // Stock del producto
                CategoryId.Create(Guid.NewGuid()), // Categoría
                userId // Usuario propietario
            )
        };

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(productDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(productDtos)).Returns(productEntities);

            // Act
            var result = await _repository.GetAllAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Nombre del Producto", result[0].ProductName.Value);
        }

        [Fact]
        public async Task GetAllAsync_ShouldMapDtoCorrectly()
        {
            // Arrange
            var userId = ProductUserId.Create(Guid.NewGuid());

            var productDtos = new List<GetProductDto>
        {
            new() { ProductUserId = userId.Value, ProductAvilability = "Disponible", ProductPrice = 150 }
        };

            var expectedEntity = new ProductEntity(
                ProductId.Create(Guid.NewGuid()), // ID del producto
                ProductName.Create("Nombre del Producto"), // Nombre del producto
                ProductImage.FromBase64("c29tZSBkYXRhIGVuIGJhc2U2NA=="), // Imagen del producto
                ProductPrice.Create(99.99m), // Precio del producto
                ProductDescription.Create("Descripción breve del producto"), // Descripción del producto
                ProductAvilability.Disponible, // Estado de disponibilidad
                ProductStock.Create(50), // Stock del producto
                CategoryId.Create(Guid.NewGuid()), // Categoría
                userId // Usuario propietario
            );

            var mockCursor = new Mock<IAsyncCursor<GetProductDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(productDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<ProductEntity>>(),
                    It.IsAny<FindOptions<ProductEntity, GetProductDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<ProductEntity>>(productDtos)).Returns(new List<ProductEntity> { expectedEntity });

            // Act
            var result = await _repository.GetAllAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Nombre del Producto", result[0].ProductName.Value);
        }







    }
}
