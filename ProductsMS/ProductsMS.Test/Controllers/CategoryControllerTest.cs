using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ProductsMS.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductsMS.Application.Category.Queries;
using Xunit;
using ProductosMs.Controllers;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Test.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<CategoryController>> _mockLogger;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_mockLogger.Object, _mockMediator.Object);
        }



        [Fact]
        public async Task GetCategory_ShouldReturnOk_WhenCategoryExists()
        {
            // Arrange
            var expectedCategory = new GetCategoryDto
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Furniture",
                IsDeleted = false
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await _controller.GetCategory(expectedCategory.CategoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var actualCategory = Assert.IsType<GetCategoryDto>(okResult.Value);
            Assert.Equal(expectedCategory.CategoryId, actualCategory.CategoryId);
            Assert.Equal(expectedCategory.CategoryName, actualCategory.CategoryName);
            Assert.False(actualCategory.IsDeleted);
        }


        [Fact]
        public async Task GetCategory_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected database error"));

            // Act
            var result = await _controller.GetCategory(Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, badRequestResult.StatusCode);
            Assert.Equal("An error occurred while trying to search an Category", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCategoryName_ShouldReturnNotFound_WhenNameDoesNotExist()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CategoryNotFoundException("Category not found"));

            // Act
            var result = await _controller.GetCategoryName("Nonexistent");

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Category not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategoryName_ShouldReturnOk_WhenCategoryExists()
        {
            // Arrange
            var expectedCategory = new GetCategoryDto
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Books",
                IsDeleted = false
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await _controller.GetCategoryName(expectedCategory.CategoryName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var actualCategory = Assert.IsType<GetCategoryDto>(okResult.Value);
            Assert.Equal(expectedCategory.CategoryId, actualCategory.CategoryId);
            Assert.Equal(expectedCategory.CategoryName, actualCategory.CategoryName);
            Assert.False(actualCategory.IsDeleted);
        }




        [Fact]

        public async Task GetAllCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GetCategoryDto>());

            var result = await _controller.GetAllCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsType<List<GetCategoryDto>>(okResult.Value);

            Assert.Empty(categories); // 🔥 La lista debe estar vacía
        }

        [Fact]
        public async Task GetCategoryByName_ShouldReturnNotFound_WhenCategoryNameDoesNotExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CategoryNotFoundException("Category name not found"));

            var result = await _controller.GetCategoryName("NonExistentCategory");

            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Category name not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategoryByName_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetCategoryName("Electronics");

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("An error occurred while trying to search an Category", internalServerErrorResult.Value);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetAllCategories();

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("An error occurred while trying to search Category", internalServerErrorResult.Value);
        }
        [Fact]
        public async Task GetAllCategories_ShouldReturnCorrectCategoryData_WhenCategoriesExist()
        {
            // Arrange
            var expectedCategories = new List<GetCategoryDto>
            {
                new GetCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = "Electronics", IsDeleted = false },
                new GetCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = "Furniture", IsDeleted = false }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var actualCategories = Assert.IsType<List<GetCategoryDto>>(okResult.Value);
            Assert.All(actualCategories, category =>
            {
                Assert.Contains(expectedCategories, c => c.CategoryId == category.CategoryId);
                Assert.Contains(expectedCategories, c => c.CategoryName == category.CategoryName);
                Assert.False(category.IsDeleted);
            });
        }




        [Fact]
        public async Task GetCategory_ShouldReturnBadRequest_WhenIdIsInvalid()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidAttributeException("Invalid category ID"));

            var result = await _controller.GetCategory(Guid.Empty); // 🔥 `Guid.Empty` representa un ID inválido

            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid category ID", badRequestResult.Value);
        }


        [Fact]

        public async Task GetAllCategories_ShouldReturnOk_WhenCategoriesExist()
        {
            var expectedCategories = new List<GetCategoryDto>
            {
                new GetCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = "Electronics", IsDeleted = false }
            };

          



            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategories);

            var result = await _controller.GetAllCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // 🔹 Convertir `CategoryEntity` a `GetCategoryDto` para comparar correctamente
            var actualCategories = (okResult.Value as List<GetCategoryDto>)?.Select(c => new GetCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                IsDeleted = c.IsDeleted
            }).ToList();

            Assert.Collection(actualCategories,
                category => Assert.Equal(expectedCategories[0].CategoryId, category.CategoryId)
               
            );
        }

        [Fact]
        public async Task GetCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new CategoryNotFoundException("Category not found"));

            var result = await _controller.GetCategory(Guid.NewGuid());

            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Category not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategoryName_ShouldReturnBadRequest_WhenNameIsInvalid()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidAttributeException("Invalid category name"));

            var result = await _controller.GetCategoryName("InvalidName");

            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid category name", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetCategory(Guid.NewGuid());

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("An error occurred while trying to search an Category", internalServerErrorResult.Value);
        }
    }

}
