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
        public async Task GetAllCategories_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

      
        [Fact]
        public async Task GetAllCategories_ShouldReturnOk_WhenCategoriesExist()
        {
            var expectedCategories = new List<GetCategoryDto> { new GetCategoryDto { CategoryId = Guid.NewGuid() } };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ReturnsAsync(expectedCategories);

            var result = await _controller.GetAllCategories() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedCategories, result.Value);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnNotFound_WhenNoCategoriesExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new CategoryNotFoundException("No hay categorías disponibles."));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Categoría no encontrada", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllCategories_ShouldThrowNullAttributeException_WhenDataIsNull()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new NullAttributeException("Datos nulos detectados."));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo nulo detectado", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllCategories_ShouldThrowInvalidAttributeException_WhenDataIsInvalid()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new InvalidAttributeException("Atributos inválidos detectados."));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo inválido detectado", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllCategories_ShouldThrowTimeoutException_WhenRequestTimesOut()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new TimeoutException("Tiempo de espera excedido."));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllCategories_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllCategories_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllCategories_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetAllCategories() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar buscar categorías.", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategoryName_ShouldReturnOk_WhenCategoryExists()
        {
            var categoryName = "Tecnología";
            var expectedCategory = new GetCategoryDto { CategoryName = categoryName };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ReturnsAsync(expectedCategory);

            var result = await _controller.GetCategoryName(categoryName) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedCategory, result.Value);
        }

        [Fact]
        public async Task GetCategoryName_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var categoryName = "NoExistente";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ThrowsAsync(new CategoryNotFoundException("Categoría no encontrada"));

            var result = await _controller.GetCategoryName(categoryName) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Categoría no encontrada", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategoryName_ShouldThrowNullAttributeException_WhenDataIsNull()
        {
            var categoryName = "Electrónica";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ThrowsAsync(new NullAttributeException("Atributo nulo detectado"));

            var result = await _controller.GetCategoryName(categoryName) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo nulo detectado", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategoryName_ShouldThrowInvalidAttributeException_WhenDataIsInvalid()
        {
            var categoryName = "CategoríaErronea";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ThrowsAsync(new InvalidAttributeException("Atributo inválido detectado"));

            var result = await _controller.GetCategoryName(categoryName) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo inválido detectado", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategoryName_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var categoryName = "Ropa";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetCategoryName(categoryName) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategoryName_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var categoryName = "Muebles";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetCategoryName(categoryName) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategoryName_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var categoryName = "Electrodomésticos";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetCategoryName(categoryName) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar buscar la categoría.", result.Value.ToString());
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
        public async Task GetCategory_ShouldThrowNullAttributeException_WhenDataIsNull()
        {
            var categoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), default))
                .ThrowsAsync(new NullAttributeException("Atributo nulo detectado"));

            var result = await _controller.GetCategory(categoryId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo nulo detectado", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategory_ShouldThrowInvalidAttributeException_WhenDataIsInvalid()
        {
            var categoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), default))
                .ThrowsAsync(new InvalidAttributeException("Atributo inválido detectado"));

            var result = await _controller.GetCategory(categoryId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo inválido detectado", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategory_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var categoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetCategory(categoryId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategory_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var categoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetCategory(categoryId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task GetCategory_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var categoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetCategory(categoryId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar buscar la categoría.", result.Value.ToString());
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
            Assert.Equal("Ocurrió un error inesperado al intentar buscar la categoría.", badRequestResult.Value);
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
            Assert.Equal("Categoría no encontrada: Category not found", notFoundResult.Value);
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
            Assert.Equal("Categoría no encontrada: Category name not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategoryByName_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetCategoryName("Electronics");

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("Ocurrió un error inesperado al intentar buscar la categoría.", internalServerErrorResult.Value);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetAllCategories();

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("Ocurrió un error inesperado al intentar buscar categorías.", internalServerErrorResult.Value);
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
            Assert.Equal("Atributo inválido detectado: Invalid category ID", badRequestResult.Value);
        }


      

        [Fact]
        public async Task GetCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new CategoryNotFoundException("Category not found"));

            var result = await _controller.GetCategory(Guid.NewGuid());

            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Categoría no encontrada: Category not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategoryName_ShouldReturnBadRequest_WhenNameIsInvalid()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryNameQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidAttributeException("Invalid category name"));

            var result = await _controller.GetCategoryName("InvalidName");

            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Atributo inválido detectado: Invalid category name", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetCategory(Guid.NewGuid());

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("Ocurrió un error inesperado al intentar buscar la categoría.", internalServerErrorResult.Value);
        }
    }

}
