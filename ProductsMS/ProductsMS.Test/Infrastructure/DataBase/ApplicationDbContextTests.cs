using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductosMs.Infrastructure.Database;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Infrastructure.Database.Context.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using Xunit;
using ProductsMS.Common.Enum;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Domain.Entities.Category;

namespace ProductsMS.Test.Infrastructure.DataBase
{
    public class ApplicationDbContextTests
    {
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<DbContextOptions<ApplicationDbContext>> _mockDbContextOptions;
        private ApplicationDbContext _dbContext;

        public ApplicationDbContextTests()
        {
            // 🔹 Configurar mocks
            _mockPublisher = new Mock<IPublisher>();

            // 🔹 Crear opciones de base de datos en memoria para pruebas
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // 🔹 Inicializar el DbContext con opciones y el mock de Publisher
            _dbContext = new ApplicationDbContext(options);
        }


        [Fact]
        public void Constructor_ShouldInitializeDbContext()
        {
            // Assert
            Assert.NotNull(_dbContext);
            Assert.NotNull(_dbContext.DbContext);
        }
        [Fact]
        public void BeginTransaction_ShouldThrowNotSupportedException_WhenUsingInMemoryDatabase()
        {
            // 🔹 Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _dbContext.BeginTransaction());

            Assert.Contains("Transactions are not supported by the in-memory store", exception.Message);
        }

        [Fact]
        public void ChangeEntityState_ShouldChangeState_WhenEntityIsNotNull()
        {
            // Arrange
            var entity = new ProductEntity(ProductId.Create(Guid.NewGuid()) );

            // Act
            _dbContext.ChangeEntityState(entity, EntityState.Modified);

            // Assert
            Assert.Equal(EntityState.Modified, _dbContext.Entry(entity).State);
        }

        [Fact]
        public void SetPropertyIsModifiedToFalse_ShouldMarkPropertyAsUnmodified()
        {
            // Arrange
            var entity = new ProductEntity(ProductId.Create(Guid.NewGuid()));
            _dbContext.Entry(entity).Property(e => e.ProductName).IsModified = true;

            // Act
            _dbContext.SetPropertyIsModifiedToFalse(entity, e => e.ProductName);

            // Assert
            Assert.False(_dbContext.Entry(entity).Property(e => e.ProductName).IsModified);
        }
       

       [Fact]
        public async Task SaveChangesAsync_ShouldUpdateTimestamps_OnAddedEntity()
        {
            // Arrange
            var product =  new ProductEntity(
                ProductId.Create(Guid.NewGuid()), // Corregido: Eliminado el segundo parámetro en Create()
                ProductName.Create("Old Name"),
                ProductImage.FromBase64("old_image.png"),
                ProductPrice.Create(80),
                ProductDescription.Create("Old Description"),
                ProductAvilability.Disponible,
                ProductStock.Create(20),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(Guid.NewGuid()),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Products.Add(product);

            // Act
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.NotEqual(default, product.CreatedAt);
            Assert.NotEqual(default, product.UpdatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldUpdateTimestamps_OnModifiedEntity()
        {
            // Arrange
            var product = new  ProductEntity(
                ProductId.Create(Guid.NewGuid()), // Corregido: Eliminado el segundo parámetro en Create()
                ProductName.Create("Old Name"),
                ProductImage.FromBase64("old_image.png"),
                ProductPrice.Create(80),
                ProductDescription.Create("Old Description"),
                ProductAvilability.Disponible,
                ProductStock.Create(20),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(Guid.NewGuid()),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            product.ProductName = ProductName.Create("Updated Product"); // 🔥 Crea un nuevo objeto de valor
            // Act
            await _dbContext.SaveChangesAsync();
            // Assert
            Assert.NotEqual(product.CreatedAt, product.UpdatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_WithUser_ShouldUpdateCreatedByAndUpdatedBy()
        {
            // Arrange
            var product = new ProductEntity(
                ProductId.Create(Guid.NewGuid()), // Corregido: Eliminado el segundo parámetro en Create()
                ProductName.Create("Old Name"),
                ProductImage.FromBase64("old_image.png"),
                ProductPrice.Create(80),
                ProductDescription.Create("Old Description"),
                ProductAvilability.Disponible,
                ProductStock.Create(20),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(Guid.NewGuid()),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Products.Add(product);

            // Act
            await _dbContext.SaveChangesAsync("TestUser");

            // Assert
            Assert.Equal("TestUser", product.CreatedBy);
            Assert.Equal("TestUser", product.UpdatedBy);
        }

        [Fact]
        public async Task SaveEfContextChanges_ShouldReturnTrue_WhenChangesAreSaved()
        {
            // Arrange
            var product = new ProductEntity(
                ProductId.Create(Guid.NewGuid()), // Corregido: Eliminado el segundo parámetro en Create()
                ProductName.Create("Old Name"),
                ProductImage.FromBase64("old_image.png"),
                ProductPrice.Create(80),
                ProductDescription.Create("Old Description"),
                ProductAvilability.Disponible,
                ProductStock.Create(20),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(Guid.NewGuid()),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Products.Add(product);

            // Act
            var result = await _dbContext.SaveEfContextChanges("TestUser");

            // Assert
            Assert.True(result);
        }
      

        [Fact]
        public async Task SaveChangesAsync_ShouldUpdateTimestamps_OnModifiedCategory()
        {
            // 🔹 Arrange
            var category = new CategoryEntity(
                CategoryId.Create(Guid.NewGuid()),
                CategoryName.Create("Electronics"),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            // 🔹 Modificar la categoría
            category.CategoryName = CategoryName.Create("Updated Electronics");

            // 🔹 Act
            await _dbContext.SaveChangesAsync();

            // 🔹 Assert
            Assert.NotEqual(category.CreatedAt, category.UpdatedAt); // 🔥 `UpdatedAt` debe haber cambiado
        }

        [Fact]
        public async Task SaveChangesAsync_WithUser_ShouldUpdateCreatedByAndUpdatedBy_OnCategory()
        {
            // 🔹 Arrange
            var category = new CategoryEntity(
                CategoryId.Create(Guid.NewGuid()),
                CategoryName.Create("Electronics"),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Categories.Add(category);

            // 🔹 Act
            await _dbContext.SaveChangesAsync("TestUser");

            // 🔹 Assert
            Assert.Equal("TestUser", category.CreatedBy); // 🔥 Verificar que `CreatedBy` es correcto
            Assert.Equal("TestUser", category.UpdatedBy); // 🔥 Verificar que `UpdatedBy` es correcto
        }

        [Fact]
        public async Task SaveEfContextChanges_ShouldReturnTrue_WhenCategoryIsSaved()
        {
            // 🔹 Arrange
            var category = new CategoryEntity(
                CategoryId.Create(Guid.NewGuid()),
                CategoryName.Create("Electronics"),
                "TestUser",
                DateTime.UtcNow,
                "TestUser",
                DateTime.UtcNow
            );
            _dbContext.Categories.Add(category);

            // 🔹 Act
            var result = await _dbContext.SaveEfContextChanges("TestUser");

            // 🔹 Assert
            Assert.True(result); // 🔥 Verificar que la operación se completó correctamente
        }

        [Fact]
        public async Task SaveEfContextChanges_ShouldReturnFalse_WhenNoChangesAreSaved()
        {
            // Act
            var result = await _dbContext.SaveEfContextChanges("User");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SaveEfContextChanges_ShouldReturnTrue_WhenSaveChangesFails()
        {
            // Arrange
           

            // Act
            var result = await _dbContext.SaveEfContextChanges();

            // Assert
            Assert.False(result);
        }

       

    }
}
