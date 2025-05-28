using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Core.Database;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;


using Xunit;
using Moq;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MongoDB.Driver.Linq;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Test.Infrastructure.Repositories
{




    public class ProductRepositoryTests
    {
        private readonly Mock<IApplicationDbContext> _mockDbContext = new();
        private readonly Mock<IMapper> _mockMapper = new();

        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            _repository = new ProductRepository(_mockDbContext.Object, _mockMapper.Object);
        }

        // 🚀 PRUEBA: Agregar producto
        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            var product = new ProductEntity(
                ProductId.Create(Guid.NewGuid()),
                ProductName.Create("Producto Test"),
                ProductImage.FromBase64("https://example.com/img.jpg"),
                ProductPrice.Create(99.99m),
                ProductDescription.Create("Descripción de prueba"),
                ProductAvilability.Disponible,
                ProductStock.Create(10),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(Guid.NewGuid())
            );

            // Simular `EntityEntry<ProductEntity>` para evitar errores
            var mockEntityEntry = new Mock<EntityEntry<ProductEntity>>();
            mockEntityEntry.Setup(e => e.Entity).Returns(product);

            // Simular `AddAsync` correctamente con `EntityEntry<ProductEntity>`
            _mockDbContext.Setup(db => db.Products.AddAsync(product, default))
                .ReturnsAsync((EntityEntry<ProductEntity>)null); // Devolver `null` porque EF solo rastrea, no usa retorno en pruebas

            // Simular `SaveEfContextChanges`
            _mockDbContext.Setup(db => db.SaveEfContextChanges(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _repository.AddAsync(product);

            // Verificar que `AddAsync` fue llamado
            _mockDbContext.Verify(db => db.Products.AddAsync(product, default), Times.Once);

            // Verificar que `SaveEfContextChanges` fue llamado
            _mockDbContext.Verify(db => db.SaveEfContextChanges(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

   

       
       


        // 🚨 PRUEBA: Eliminar producto

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct_WhenExists()
        {
            var productId = ProductId.Create(Guid.NewGuid());
            var existingProduct = new ProductEntity(
                productId,
                ProductName.Create("Producto Test"),
                ProductImage.FromBase64("https://example.com/img.jpg"),
                ProductPrice.Create(99.99m),
                ProductDescription.Create("Descripción de prueba"),
                ProductAvilability.Disponible,
                ProductStock.Create(10),
                CategoryId.Create(Guid.NewGuid()),
                ProductUserId.Create(Guid.NewGuid())
            );

            // Simular `FindAsync()`
            _mockDbContext.Setup(db => db.Products.FindAsync(productId))
                .ReturnsAsync(existingProduct);

            // Simular `SaveEfContextChanges()`
            _mockDbContext.Setup(db => db.SaveEfContextChanges(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _repository.DeleteAsync(productId);

          

            // Verificaciones
            _mockDbContext.Verify(db => db.Products.FindAsync(productId), Times.Once);
            _mockDbContext.Verify(db => db.SaveEfContextChanges(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        // 🔄 PRUEBA: Actualizar producto
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenExists()
        {
            var product = new ProductEntity();

            _mockDbContext.Setup(db => db.Products.Update(product)).Verifiable();

            var result = await _repository.UpdateAsync(product);

            Assert.NotNull(result);
            _mockDbContext.Verify(db => db.Products.Update(product), Times.Once);
        }




    }

}
