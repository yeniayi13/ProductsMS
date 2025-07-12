using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Infrastructure.Repositories;
namespace ProductsMS.Test.Infrastructure.Repositories
{


    public class CategoryRepositoryTests
    {
        private readonly Mock<IMongoCollection<CategoryEntity>> _mockCollection = new();
        private readonly Mock<IMapper> _mockMapper = new();


        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            _repository = new CategoryRepository(_mockCollection.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var categoryId = CategoryId.Create(Guid.NewGuid());
            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CategoryEntity>>(),
                It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(categoryId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var categoryId = CategoryId.Create(Guid.NewGuid());
            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _repository.GetByIdAsync(categoryId));
        }
        [Fact]
        public async Task GetByNameAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var categoryName = CategoryName.Create("Electronics");
            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _repository.GetByNameAsync(categoryName));
        }
        [Fact]
        public async Task GetAllAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _repository.GetAllAsync());
        }

        [Fact]
        public async Task GetAllAsync_ShouldHandleLargeDataset()
        {
            // Arrange
            var categoryDtos = Enumerable.Range(1, 1000)
                .Select(i => new GetCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = $"Category_{i}" })
                .ToList();
            var listCategory = new List<GetCategoryDto>();
            var categoryEntities = categoryDtos.Select(dto =>
                new CategoryEntity(CategoryId.Create(dto.CategoryId), CategoryName.Create(dto.CategoryName))).ToList();

            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(categoryDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<CategoryEntity>>(categoryDtos)).Returns(categoryEntities);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1000, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenExists()
        {
            // Arrange
            var categoryId = CategoryId.Create(Guid.NewGuid());
            var categoryDto = new GetCategoryDto
            {
                CategoryId = categoryId.Value,
                CategoryName = "Electronics"
            };

            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();

            // Simula que hay resultados en el cursor
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)  // Primera iteración: hay datos
                .ReturnsAsync(false); // Segunda iteración: no hay más datos

            mockCursor.SetupGet(c => c.Current).Returns(new[] { categoryDto });

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var expectedEntity = new CategoryEntity(CategoryId.Create(categoryDto.CategoryId), CategoryName.Create(categoryDto.CategoryName));
            _mockMapper.Setup(m => m.Map<CategoryEntity>(categoryDto)).Returns(expectedEntity);

            // Act
            var result = await _repository.GetByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Electronics", result.CategoryName.Value);
        }

        [Fact]
        
        public async Task GetByNameAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var categoryName = CategoryName.Create("NonExistentCategory");
            //var categoryDto = new GetCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = "NonExistentCategory" };
            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    default))
                .ReturnsAsync(mockCursor.Object);
          
            //_mockMapper.Setup(m => m.Map<CategoryEntity>(categoryDto)).Returns(new CategoryEntity());

            // Act
            var result = await _repository.GetByNameAsync(categoryName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnCategory_WhenExists()
        {
            // Arrange
            var categoryName = CategoryName.Create("Electronics");
            var categoryDto = new GetCategoryDto
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Electronics"
            };

            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)  // Simula que hay resultados disponibles
                .ReturnsAsync(false); // Simula que ya no hay más datos

            mockCursor.SetupGet(c => c.Current).Returns(new[] { categoryDto });

            mockCursor.SetupGet(c => c.Current).Returns(new[] { categoryDto });

            // Configurar el mock de la colección para que retorne el cursor
            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Configurar el mock del mapper para convertir DTO en entidad
            var expectedEntity = new CategoryEntity(CategoryId.Create(categoryDto.CategoryId), CategoryName.Create(categoryDto.CategoryName));
            _mockMapper.Setup(m => m.Map<CategoryEntity>(categoryDto)).Returns(expectedEntity);

            // Act
            var result = await _repository.GetByNameAsync(categoryName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Electronics", result.CategoryName.Value);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedEntities_WhenCategoriesExist()
        {
             var id = Guid.NewGuid();

            // Arrange
            var categoryDtos = new List<GetCategoryDto>
            {
                new() { CategoryId =id, CategoryName = "TestCategory" }
            };

            var categoryEntities = new List<CategoryEntity>
            {
                new(CategoryId.Create(id), CategoryName.Create("TestCategory"))
            };

            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(c => c.Current).Returns(categoryDtos);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    default))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<List<CategoryEntity>>(categoryDtos)).Returns(categoryEntities);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestCategory", result[0].CategoryName.Value);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();

            // Simula que `MoveNextAsync` devuelve `false` desde el inicio, indicando que no hay datos
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // `Current` nunca debería tener datos porque `MoveNextAsync` siempre es `false`
            mockCursor.SetupGet(c => c.Current).Returns(new List<GetCategoryDto>());

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<CategoryEntity>>(),
                    It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Simula que el mapeo devuelve una lista vacía
            _mockMapper.Setup(m => m.Map<List<CategoryEntity>>(It.IsAny<List<GetCategoryDto>>()))
                .Returns(new List<CategoryEntity>());

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Verifica que la lista está vacía
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnMappedEntity_WhenCategoryExists()
        {
            // Arrange
            var categoryName = CategoryName.Create("ExistingCategory");
            var categoryDto = new GetCategoryDto { CategoryId = new Guid(), CategoryName = "ExistingCategory" };
            var categoryEntity = new CategoryEntity(CategoryId.Create(categoryDto.CategoryId), CategoryName.Create("ExistingCategory"));

            var mockCursor = new Mock<IAsyncCursor<GetCategoryDto>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(true);
            mockCursor.SetupGet(c => c.Current).Returns(new List<GetCategoryDto> { categoryDto });

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CategoryEntity>>(),
                It.IsAny<FindOptions<CategoryEntity, GetCategoryDto>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            _mockMapper.Setup(m => m.Map<CategoryEntity>(categoryDto)).Returns(categoryEntity);

            // Act
            var result = await _repository.GetByNameAsync(categoryName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ExistingCategory", result.CategoryName.Value);
        }
    }
}
