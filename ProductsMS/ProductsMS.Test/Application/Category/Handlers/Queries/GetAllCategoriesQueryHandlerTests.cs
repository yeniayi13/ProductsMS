using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ProductosMs.Application.Category.Handlers.Queries;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Exceptions;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using Xunit;

using AutoMapper;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Test.Application.Category.Handlers.Queries
{
  
    public class GetAllCategoriesQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllCategoriesQueryHandler _handler;

        public GetAllCategoriesQueryHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllCategoriesQueryHandler(_categoryRepositoryMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldReturnCategoryList_WhenCategoriesExist()
        {
            // Arrange
            var request = new GetAllProductsQuery();
            var categoryList = new List<CategoryEntity>
            {
                new CategoryEntity ( CategoryId.Create(Guid.NewGuid()), CategoryName.Create( "Category 1") ),
                new CategoryEntity ( CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Category 2") )
            };
            var categoryDtoList = new List<GetCategoryDto>
            {
                new GetCategoryDto { CategoryId = categoryList[0].CategoryId.Value, CategoryName = categoryList[0].CategoryName.Value },
                new GetCategoryDto { CategoryId = categoryList[1].CategoryId.Value, CategoryName = categoryList[1].CategoryName.Value }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(categoryList);

            _mapperMock.Setup(m => m.Map<List<GetCategoryDto>>(categoryList))
                .Returns(categoryDtoList);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryDtoList.Count, result.Count);
            Assert.Equal(categoryDtoList[0].CategoryName, result[0].CategoryName);
            Assert.Equal(categoryDtoList[1].CategoryName, result[1].CategoryName);
            _categoryRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<GetCategoryDto>>(categoryList), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoriesAreEmpty()
        {
            // Arrange
            var request = new GetAllProductsQuery();

            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<CategoryEntity>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() => _handler.Handle(request, CancellationToken.None));

            Assert.Equal("Categories are empty", exception.Message);
            _categoryRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}
