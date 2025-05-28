using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductosMs.Controllers;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using Xunit;
using ProductosMs.Application.Products.Queries;
using ProductsMS.Infrastructure.Exceptions;

namespace ProductsMS.Test.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockLogger.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task CreatedProduct_ShouldReturnOk_WhenProductIsCreatedSuccessfully()
        {
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var userId = Guid.NewGuid();
            var expectedCategoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(expectedCategoryId);

            var result = await _controller.CreatedProduct(createProductDto, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedCategoryId, result.Value);
        }

       /* [Fact]
        public async Task CreatedProduct_ShouldThrowArgumentNullException_WhenCreateProductDtoIsNull()
        {
            var userId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.CreatedProduct(null, userId));

            Assert.Contains("El objeto de creación de producto no puede ser nulo.", exception.Message);
        }*/

        [Fact]
        public async Task CreatedProduct_ShouldThrowProductNotFoundException_WhenProductDoesNotExist()
        {
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new ProductNotFoundException("El producto no fue encontrado."));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Producto no encontrado", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldThrowValidatorException_WhenValidationFails()
        {
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new ValidatorException("Error de validación"));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Error de validación", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar crear el producto.", result.Value.ToString());
        }
        [Fact]
        public async Task CreatedProduct_ShouldReturnBadRequest_WhenCreateProductDtoHasInvalidValues()
        {
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto { /* Valores incorrectos */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new InvalidAttributeException("Uno o más atributos son inválidos."));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Error de atributo inválido: Uno o más atr", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldThrowInvalidOperationException_WhenCreationFails()
        {
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new InvalidOperationException("El producto no pudo ser creado correctamente."));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Operación inválida", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldThrowArgumentNullException_WhenCreateProductDtoHasNullValues()
        {
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto { /* Contiene valores nulos */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new NullAttributeException("Hay atributos nulos en la creación del producto."));

            var result = await _controller.CreatedProduct(createProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Error de atributo nulo: Hay atributos nulo", result.Value.ToString());
        }

        [Fact]
        public async Task CreatedProduct_ShouldHandleMultipleConcurrentCreations()
        {
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var expectedCategoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(expectedCategoryId);

            var tasks = new Task<IActionResult>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _controller.CreatedProduct(createProductDto, userId);
            }

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
                Assert.Equal(200, okResult.StatusCode);
            });
        }

        [Fact]
        public async Task CreatedProduct_ShouldHandleSlowResponse()
        {
            var userId = Guid.NewGuid();
            var createProductDto = new CreateProductDto { /* Datos simulados */ };
            var expectedCategoryId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(() =>
                {
                    Task.Delay(5000).Wait(); // Simula una respuesta lenta
                    return expectedCategoryId;
                });

            var result = await _controller.CreatedProduct(createProductDto, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedCategoryId, result.Value);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WhenProductsExist()
        {
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto> { new GetProductDto{ProductUserId = userId} };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ReturnsAsync(expectedProducts);

            var result = await _controller.GetAllProducts(userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProducts, result.Value);
        }

      /*  [Fact]
        public async Task GetAllProducts_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _controller.GetAllProducts(Guid.Empty));

            Assert.Contains("El ID de usuario no puede estar vacío.", exception.Message);
        }*/

        [Fact]
        public async Task GetAllProducts_ShouldReturnNotFound_WhenNoProductsExist()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ReturnsAsync(new List<GetProductDto>()); // Simula lista vacía

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("No products found.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowTimeoutException_WhenRequestTimesOut()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ThrowsAsync(new TimeoutException("Tiempo de espera excedido."));

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(408, result.StatusCode);
            Assert.Contains("Tiempo de espera excedido", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(503, result.StatusCode);
            Assert.Contains("Servicio no disponible", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar obtener los productos.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WhenLargeNumberOfProductsExist()
        {
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto>();

            for (int i = 0; i < 1000; i++) // Simula una lista grande de productos
            {
                expectedProducts.Add(new GetProductDto { ProductUserId = userId });
            }

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ReturnsAsync(expectedProducts);

            var result = await _controller.GetAllProducts(userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProducts.Count, (result.Value as List<GetProductDto>).Count);
        }

        [Fact]
        public async Task GetAllProducts_ShouldHandleMultipleConcurrentRequests()
        {
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto> { new GetProductDto { ProductUserId = userId } };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ReturnsAsync(expectedProducts);

            var tasks = new Task<IActionResult>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _controller.GetAllProducts(userId);
            }

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
                Assert.Equal(200, okResult.StatusCode);
            });
        }

        [Fact]
        public async Task GetAllProducts_ShouldHandleSlowResponse()
        {
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto> { new GetProductDto { ProductUserId = userId } };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ReturnsAsync(() =>
                {
                    Task.Delay(5000).Wait(); // Simula una respuesta lenta
                    return expectedProducts;
                });

            var result = await _controller.GetAllProducts(userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProducts, result.Value);
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowInvalidOperationException_WhenUnexpectedFailureOccurs()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), default))
                .ThrowsAsync(new InvalidOperationException("Error inesperado en la operación"));

            var result = await _controller.GetAllProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar ", result.Value.ToString());
        }

    

        [Fact]
        public async Task GetAvailableProducts_ShouldReturnOk_WhenProductsExist()
        {
            var userId = Guid.NewGuid();
            var expectedProducts = new List<GetProductDto> { new GetProductDto{ ProductUserId = userId } };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ReturnsAsync(expectedProducts);

            var result = await _controller.GetAvailableProducts(userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProducts, result.Value);
        }

       // [Fact]
      /*  public async Task GetAvailableProducts_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.GetAvailableProducts(Guid.Empty));

            Assert.Contains("El ID del usuario es requerido.", exception.Message);
        }*/

        [Fact]
        public async Task GetAvailableProducts_ShouldThrowArgumentException_WhenMinPriceIsGreaterThanMaxPrice()
        {
            var userId = Guid.NewGuid();
            var minPrice = 500m;
            var maxPrice = 100m;

            var result = await _controller.GetAvailableProducts(userId, null, minPrice, maxPrice) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El precio mínimo no puede ser mayor que el precio máximo.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldReturnNotFound_WhenNoProductsExist()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ReturnsAsync(new List<GetProductDto>());

            var result = await _controller.GetAvailableProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("No se encontraron productos disponibles.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetAvailableProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldThrowTimeoutException_WhenRequestTimesOut()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ThrowsAsync(new TimeoutException("Tiempo de espera excedido."));

            var result = await _controller.GetAvailableProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(408, result.StatusCode);
            Assert.Contains("Tiempo de espera excedido", result.Value.ToString());
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetAvailableProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.GetAvailableProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(503, result.StatusCode);
            Assert.Contains("Servicio no disponible", result.Value.ToString());
        }

        [Fact]
        public async Task GetAvailableProducts_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var userId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAvailableProductsQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetAvailableProducts(userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al obtener los productos.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldReturnOk_WhenProductsExist()
        {
            var userId = Guid.NewGuid();
            var productName = "TestProduct";
            var expectedProducts = new GetProductDto{ProductName = productName,ProductUserId = userId};

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ReturnsAsync(expectedProducts);

            var result = await _controller.GetAllNameProducts(productName, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProducts, result.Value);
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldReturnBadRequest_WhenProductNameIsEmpty()
        {
            var userId = Guid.NewGuid();

            var result = await _controller.GetAllNameProducts("", userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El nombre del producto es requerido.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldReturnNotFound_WhenNoProductsExist()
        {
            var userId = Guid.NewGuid();
            var productName = "NonExistentProduct";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ReturnsAsync((GetProductDto)null);

            var result = await _controller.GetAllNameProducts(productName, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("No se encontraron productos con ese nombre.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var userId = Guid.NewGuid();
            var productName = "TestProduct";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetAllNameProducts(productName, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldThrowTimeoutException_WhenRequestTimesOut()
        {
            var userId = Guid.NewGuid();
            var productName = "TestProduct";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ThrowsAsync(new TimeoutException("Tiempo de espera excedido."));

            var result = await _controller.GetAllNameProducts(productName, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(408, result.StatusCode);
            Assert.Contains("Tiempo de espera excedido", result.Value.ToString());
        }


        [Fact]
        public async Task GetAllNameProducts_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var userId = Guid.NewGuid();
            var productName = "TestProduct";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetAllNameProducts(productName, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            var userId = Guid.NewGuid();
            var productName = "TestProduct";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.GetAllNameProducts(productName, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(503, result.StatusCode);
            Assert.Contains("Servicio no disponible", result.Value.ToString());
        }

        [Fact]
        public async Task GetAllNameProducts_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var userId = Guid.NewGuid();
            var productName = "TestProduct";

            _mockMediator.Setup(m => m.Send(It.IsAny<GetNameProductQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetAllNameProducts(productName, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar buscar el producto.", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOk_WhenProductExists()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var expectedProduct = new GetProductDto{ProductId = productId,ProductUserId = userId};

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ReturnsAsync(expectedProduct);

            var result = await _controller.GetProduct(productId, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProduct, result.Value);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnBadRequest_WhenProductIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            var result = await _controller.GetProduct(Guid.Empty, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El ID del producto no puede estar vacío.", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldReturnBadRequest_WhenUserIdIsEmpty()
        {
            var productId = Guid.NewGuid();

            var result = await _controller.GetProduct(productId, Guid.Empty) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El ID del usuario no puede estar vacío.", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ReturnsAsync((GetProductDto)null);

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Producto no encontrado.", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldThrowTimeoutException_WhenRequestTimesOut()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new TimeoutException("Tiempo de espera excedido."));

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(408, result.StatusCode);
            Assert.Contains("Tiempo de espera excedido", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(503, result.StatusCode);
            Assert.Contains("Servicio no disponible", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar obtener el producto.", result.Value.ToString());
        }
        [Fact]
        public async Task GetProduct_ShouldReturnBadRequest_WhenUserIdIsInvalid()
        {
            var productId = Guid.NewGuid();
            var invalidUserId = Guid.NewGuid(); // Simulación de un ID inválido

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new ArgumentException("El ID del usuario es inválido."));

            var result = await _controller.GetProduct(productId, invalidUserId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Parámetro inválido", result.Value.ToString());
        }

        [Fact]
        public async Task GetProduct_ShouldHandleMultipleConcurrentRequests()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var expectedProduct = new GetProductDto { ProductId = productId, ProductUserId = userId };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ReturnsAsync(expectedProduct);

            var tasks = new Task<IActionResult>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _controller.GetProduct(productId, userId);
            }

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
                Assert.Equal(200, okResult.StatusCode);
            });
        }

        [Fact]
        public async Task GetProduct_ShouldHandleSlowResponse()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var expectedProduct = new GetProductDto { ProductId = productId, ProductUserId = userId };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ReturnsAsync(() =>
                {
                    Task.Delay(5000).Wait(); // Simula una respuesta lenta
                    return expectedProduct;
                });

            var result = await _controller.GetProduct(productId, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedProduct, result.Value);
        }

        [Fact]
        public async Task GetProduct_ShouldThrowInvalidOperationException_WhenUnexpectedFailureOccurs()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ThrowsAsync(new InvalidOperationException("Error inesperado en la operación"));

            var result = await _controller.GetProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado", result.Value.ToString());
        }

       

        [Fact]
        public async Task UpdateProduct_ShouldReturnOk_WhenProductIsUpdatedSuccessfully()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(productId);

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(productId, result.Value);
        }

       /* [Fact]
        public async Task UpdateProduct_ShouldThrowArgumentNullException_WhenUpdateProductDtoIsNull()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.UpdateProduct(productId, null, userId));

            Assert.Contains("Los datos de actualización no pueden ser nulos.", exception.Message);
        }*/

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenProductIdIsEmpty()
        {
            var userId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            var result = await _controller.UpdateProduct(Guid.Empty, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El ID del producto no puede estar vacío.", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(503, result.StatusCode);
            Assert.Contains("Servicio no disponible", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al actualizar el producto.", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenUpdateProductDtoHasInvalidValues()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Valores incorrectos */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ThrowsAsync(new InvalidAttributeException("Uno o más atributos son inválidos."));

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo inválido", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowInvalidOperationException_WhenUpdateFails()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ThrowsAsync(new InvalidOperationException("El producto no pudo ser actualizado correctamente."));

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Operación inválida", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowArgumentNullException_WhenUpdateProductDtoHasNullValues()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Contiene valores nulos */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ThrowsAsync(new NullAttributeException("Hay atributos nulos en la actualización del producto."));

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Atributo nulo", result.Value.ToString());
        }

        [Fact]
        public async Task UpdateProduct_ShouldHandleMultipleConcurrentUpdates()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(productId);

            var tasks = new Task<IActionResult>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _controller.UpdateProduct(productId, updateProductDto, userId);
            }

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
                Assert.Equal(200, okResult.StatusCode);
            });
        }

        [Fact]
        public async Task UpdateProduct_ShouldHandleSlowResponse()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto { /* Datos simulados */ };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(() =>
                {
                    Task.Delay(5000).Wait(); // Simula una respuesta lenta
                    return productId;
                });

            var result = await _controller.UpdateProduct(productId, updateProductDto, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(productId, result.Value);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnBadRequest_WhenProductIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            var result = await _controller.DeleteProduct(Guid.Empty, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El ID del producto no puede estar vacío.", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnBadRequest_WhenUserIdIsEmpty()
        {
            var productId = Guid.NewGuid();

            var result = await _controller.DeleteProduct(productId, Guid.Empty) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("El ID del usuario no puede estar vacío.", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new CategoryNotFoundException("Categoría no encontrada"));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Categoría no encontrada", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new UnauthorizedAccessException("No tienes permisos para esta acción."));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("No tienes permisos para acceder a este recurso.", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowTimeoutException_WhenRequestTimesOut()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new TimeoutException("Tiempo de espera excedido"));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(408, result.StatusCode);
            Assert.Contains("Tiempo de espera excedido", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowHttpRequestException_WhenAuthenticationFails()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new HttpRequestException("401 Unauthorized"));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Contains("Acceso denegado. Verifica tus credenciales.", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowHttpRequestException_WhenServiceUnavailable()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(503, result.StatusCode);
            Assert.Contains("Servicio no disponible", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new Exception("Error inesperado"));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar eliminar el producto.", result.Value.ToString());
        }
        [Fact]
        public async Task DeleteProduct_ShouldReturnBadRequest_WhenProductIdIsInvalid()
        {
            var userId = Guid.NewGuid();
            var invalidProductId = Guid.NewGuid(); // Simulación de un ID inválido

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new ArgumentException("El ID del producto es inválido."));

            var result = await _controller.DeleteProduct(invalidProductId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Parámetro inválido", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldHandleMultipleConcurrentDeletes()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ReturnsAsync(productId);

            var tasks = new Task<IActionResult>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _controller.DeleteProduct(productId, userId);
            }

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
                Assert.Equal(200, okResult.StatusCode);
            });
        }

        [Fact]
        public async Task DeleteProduct_ShouldHandleSlowResponse()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ReturnsAsync(() =>
                {
                    Task.Delay(5000).Wait(); // Simula una respuesta lenta
                    return productId;
                });

            var result = await _controller.DeleteProduct(productId, userId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(productId, result.Value);
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowInvalidOperationException_WhenDeletionFails()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new InvalidOperationException("El producto no pudo ser eliminado correctamente."));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Operación inválida", result.Value.ToString());
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenProductAlreadyDeleted()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ThrowsAsync(new ProductNotFoundException("El producto ya fue eliminado anteriormente."));

            var result = await _controller.DeleteProduct(productId, userId) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Ocurrió un error inesperado al intentar", result.Value.ToString());
        }

    }
}

