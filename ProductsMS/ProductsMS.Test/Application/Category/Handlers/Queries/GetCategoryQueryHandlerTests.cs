using System;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ProductosMs.Application.Category.Handlers.Queries;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Exceptions;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Category;
using Xunit;
using ProductsMS.Common.Dtos.Category.Response;

using AutoMapper;

namespace ProductsMS.Test.Application.Category.Handlers.Queries
{

    public class GetCategoryQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetCategoryQueryHandler _handler;

        public GetCategoryQueryHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _dbContextMock = new Mock<IApplicationDbContext>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetCategoryQueryHandler(_categoryRepositoryMock.Object, _dbContextMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var request = new GetCategoryQuery(categoryId);
            var categoryEntity =
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Test Category"));
            var expectedDto = new GetCategoryDto { CategoryId = categoryId, CategoryName = "Test Category" };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<CategoryId>()))
                .ReturnsAsync(categoryEntity);

            _mapperMock.Setup(m => m.Map<GetCategoryDto>(categoryEntity))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.CategoryId, result.CategoryId);
            Assert.Equal(expectedDto.CategoryName, result.CategoryName);
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<CategoryId>()), Times.Once);
            _mapperMock.Verify(m => m.Map<GetCategoryDto>(categoryEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryIdIsEmpty()
        {
            // Arrange
            var request = new GetCategoryQuery(Guid.Empty);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<NullAttributeException>(() =>
                    _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Category id is required", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var request = new GetCategoryQuery(categoryId);

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<CategoryId>()))
                .ReturnsAsync((CategoryEntity)null);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
                    _handler.Handle(request, CancellationToken.None));
            Assert.Equal($"Category with ID {categoryId} not found.", exception.Message);
        }
    }
}