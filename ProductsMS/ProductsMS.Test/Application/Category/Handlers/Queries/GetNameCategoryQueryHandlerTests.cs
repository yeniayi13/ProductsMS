using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ProductsMS.Application.Category.Handlers.Queries;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Exceptions;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Category;
using Xunit;
using AutoMapper;
using ProductsMS.Common.Dtos.Category.Response;

namespace ProductsMS.Test.Application.Category.Handlers.Queries
{


    public class GetNameCategoryQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetNameCategoryQueryHandler _handler;

        public GetNameCategoryQueryHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetNameCategoryQueryHandler(_categoryRepositoryMock.Object, _dbContextMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryName = "Electronics";
            var request = new GetCategoryNameQuery(categoryName);
            var categoryEntity =
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Category 1"));
            var expectedDto = new GetCategoryDto
                { CategoryId = categoryEntity.CategoryId.Value, CategoryName = categoryEntity.CategoryName.Value };

            _categoryRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<CategoryName>()))
                .ReturnsAsync(categoryEntity);

            _mapperMock.Setup(m => m.Map<GetCategoryDto>(categoryEntity))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.CategoryId, result.CategoryId);
            Assert.Equal(expectedDto.CategoryName, result.CategoryName);
            _categoryRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<CategoryName>()), Times.Once);
            _mapperMock.Verify(m => m.Map<GetCategoryDto>(categoryEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryNameIsEmpty()
        {
            // Arrange
            var request = new GetCategoryNameQuery("");

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<NullAttributeException>(() =>
                    _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Category name is required", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryName = "Nonexistent";
            var request = new GetCategoryNameQuery(categoryName);

            _categoryRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<CategoryName>()))
                .ReturnsAsync((CategoryEntity)null);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
                    _handler.Handle(request, CancellationToken.None));
            Assert.Equal($"Category with name '{categoryName}' not found.", exception.Message);
            _categoryRepositoryMock.Verify(repo => repo.GetByNameAsync(It.IsAny<CategoryName>()), Times.Once);
        }
    }
}