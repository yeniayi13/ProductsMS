using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductosMs.Application.Products.Queries;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMS.Infrastructure.Exceptions;

namespace ProductosMs.Controllers
{
    [ApiController]
    [Route("auctioneer/product")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IMediator _mediator;

        public ProductController(ILogger<ProductController> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(logger));
        }
        [Authorize(Policy = "SubastadorPolicy")]
        [HttpPost("Add-Product/{userId}")]
        public async Task<IActionResult> CreatedProduct([FromBody] CreateProductDto createProductDto, [FromRoute] Guid userId)
        {
            try
            {
                if (createProductDto == null)
                {
                    throw new ArgumentNullException(nameof(createProductDto), "El objeto de creación de producto no puede ser nulo.");
                }

                var command = new CreateProductCommand(createProductDto, userId);
                var categoryId = await _mediator.Send(command);

                if (categoryId == Guid.Empty)
                {
                    throw new InvalidOperationException("El producto no se pudo crear correctamente.");
                }

                return Ok(categoryId);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Error de argumento nulo: {e.Message}");
            }
            catch (ProductNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(404, $"Producto no encontrado: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Error de atributo nulo: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Error de atributo inválido: {e.Message}");
            }
            catch (ValidatorException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Error de validación: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(500, $"Operación inválida: {e.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado: {Message}", e.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al intentar crear el producto,  {e.Message}");
            }
        }

        [Authorize(Policy = "SubastadorPolicy")]
        [HttpGet("Product-All")]
        public async Task<IActionResult> GetAllProducts([FromQuery] Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ProductNotFoundException("El ID de usuario no puede estar vacío.");
                }

                var query = new GetAllProductQuery(userId);
                var products = await _mediator.Send(query);

                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                return Ok(products);
            }
            catch (ArgumentException e)
            {
                _logger.LogError("Error en los parámetros de entrada: {Message}", e.Message);
                return BadRequest($"Parámetro inválido: {e.Message}");
            }
            catch (ProductNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(404, $"Producto no encontrado: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Error de atributo nulo: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Error de atributo inválido: {e.Message}");
            }
            catch (TimeoutException e)
            {
                _logger.LogError("Error de tiempo de espera: {Message}", e.Message);
                return StatusCode(408, $"Tiempo de espera excedido: {e.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error de comunicación con el servidor: {Message}", e.Message);
                return StatusCode(503, $"Servicio no disponible: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado: {Message}", e.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al intentar obtener los productos, {e.Message}");
            }
        }

        [Authorize(Policy = "SubastadorPolicy")]
        [HttpGet("Product-Available")]
        public async Task<IActionResult> GetAvailableProducts(
            [FromQuery] Guid userId,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest("El ID del usuario es requerido.");
                }

                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                {
                    return BadRequest("El precio mínimo no puede ser mayor que el precio máximo.");
                }

                var query = new GetAvailableProductsQuery(userId, categoryId, minPrice, maxPrice);
                var products = await _mediator.Send(query);

                if (products == null || !products.Any())
                {
                    return NotFound("No se encontraron productos disponibles.");
                }

                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Error de parámetros de entrada: {Message}", ex.Message);
                return BadRequest($"Parámetro inválido: {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError("Tiempo de espera excedido: {Message}", ex.Message);
                return StatusCode(408, $"Tiempo de espera excedido: {ex.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Error de comunicación con el servidor: {Message}", ex.Message);
                return StatusCode(503, $"Servicio no disponible: {ex.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inesperado en GetAvailableProducts: {Message}", ex.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al obtener los productos, {ex.Message}");
            }
        }
        [Authorize(Policy = "SubastadorPolicy")]
        //[Authorize(Policy = "PostorPolicy")]
        [HttpGet("Name/Product/{name}")]
        public async Task<IActionResult> GetAllNameProducts([FromRoute] string name, [FromQuery] Guid userId)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("El nombre del producto es requerido.");
                }

                var query = new GetNameProductQuery(name, userId);
                var products = await _mediator.Send(query);

                if (products == null )
                {
                    return NotFound("No se encontraron productos con ese nombre.");
                }

                return Ok(products);
            }
            catch (ProductNotFoundException e)
            {
                _logger.LogError("Ocurrió un error al buscar el producto: {Message}", e.Message);
                return StatusCode(404, $"Producto no encontrado: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Ocurrió un error al buscar el producto: {Message}", e.Message);
                return StatusCode(400, $"Atributo nulo detectado: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Ocurrió un error al buscar el producto: {Message}", e.Message);
                return StatusCode(400, $"Atributo inválido detectado: {e.Message}");
            }
            catch (TimeoutException e)
            {
                _logger.LogError("Tiempo de espera excedido al buscar el producto: {Message}", e.Message);
                return StatusCode(408, $"Tiempo de espera excedido: {e.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error de comunicación con el servidor al buscar el producto: {Message}", e.Message);
                return StatusCode(503, $"Servicio no disponible: {e.Message}");
            }

            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (Exception e)
            {
                _logger.LogError("Ocurrió un error inesperado al buscar el producto: {Message}", e.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al intentar buscar el producto, {e.Message}");
            }
        }

        [Authorize(Policy = "SubastadorPolicy")]
        //[Authorize(Policy = "PostorPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("El ID del producto no puede estar vacío.");
                }

                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("El ID del usuario no puede estar vacío.");
                }

                var command = new GetProductQuery(id, userId);
                var product = await _mediator.Send(command);

                if (product == null)
                {
                    return NotFound("Producto no encontrado.");
                }

                return Ok(product);
            }
            catch (ArgumentException e)
            {
                _logger.LogError("Error en los parámetros de entrada: {Message}", e.Message);
                return BadRequest($"Parámetro inválido: {e.Message}");
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(404, $"Categoría no encontrada: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo nulo: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo inválido: {e.Message}");
            }
            catch (TimeoutException e)
            {
                _logger.LogError("Error de tiempo de espera: {Message}", e.Message);
                return StatusCode(408, $"Tiempo de espera excedido: {e.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error de comunicación con el servidor: {Message}", e.Message);
                return StatusCode(503, $"Servicio no disponible: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado: {Message}", e.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al intentar obtener el producto, {e.Message}");
            }
        }

        [Authorize(Policy = "SubastadorPolicy")]
        [HttpPut("Update-Product/{id}")]
        public async Task<IActionResult> UpdateProduct(
     [FromRoute] Guid id,
     [FromBody] UpdateProductDto updateProductDto,
     [FromQuery] Guid userId)
        {
            try
            {
                // Validaciones iniciales
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("El ID del producto no puede estar vacío.");
                }

                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("El ID del usuario no puede estar vacío.");
                }

                if (updateProductDto == null)
                {
                    throw new ArgumentNullException(nameof(updateProductDto), "Los datos de actualización no pueden ser nulos.");
                }

                var command = new UpdateProductCommand(id, updateProductDto, userId);
                var productId = await _mediator.Send(command);

                if (productId == Guid.Empty)
                {
                    throw new InvalidOperationException("El producto no se pudo actualizar correctamente.");
                }

                return Ok(productId);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return BadRequest($"Parámetro nulo: {e.Message}");
            }
            catch (ArgumentException e)
            {
                _logger.LogError("Error en los parámetros: {Message}", e.Message);
                return BadRequest($"Parámetro inválido: {e.Message}");
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return NotFound($"Categoría no encontrada: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return BadRequest($"Atributo nulo: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return BadRequest($"Atributo inválido: {e.Message}");
            }
            catch (TimeoutException e)
            {
                _logger.LogError("Tiempo de espera excedido: {Message}", e.Message);
                return StatusCode(408, $"Tiempo de espera excedido: {e.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error de comunicación con el servidor: {Message}", e.Message);
                return StatusCode(503, $"Servicio no disponible: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError("Error en operación: {Message}", e.Message);
                return StatusCode(500, $"Operación inválida: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado: {Message}", e.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al actualizar el producto,  {e.Message}");
            }
        }

        [Authorize(Policy = "SubastadorPolicy")]
        [HttpDelete("Delete-Product/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            try
            {
                // Validaciones iniciales
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("El ID del producto no puede estar vacío.");
                }

                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("El ID del usuario no puede estar vacío.");
                }

                var command = new DeleteProductCommand(id, userId);
                var productId = await _mediator.Send(command);

                if (productId == Guid.Empty)
                {
                    throw new InvalidOperationException("El producto no pudo ser eliminado correctamente.");
                }

                return Ok(productId);
            }
            catch (ArgumentException e)
            {
                _logger.LogError("Error en los parámetros: {Message}", e.Message);
                return BadRequest($"Parámetro inválido: {e.Message}");
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return NotFound($"Categoría no encontrada: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return BadRequest($"Atributo nulo: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return BadRequest($"Atributo inválido: {e.Message}");
            }
            catch (TimeoutException e)
            {
                _logger.LogError("Tiempo de espera excedido: {Message}", e.Message);
                return StatusCode(408, $"Tiempo de espera excedido: {e.Message}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("401"))
            {
                _logger.LogError("Error de autenticación: {Message}", e.Message);
                return StatusCode(401, "Acceso denegado. Verifica tus credenciales.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error de comunicación con el servidor: {Message}", e.Message);
                return StatusCode(503, $"Servicio no disponible: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError("Error en la operación: {Message}", e.Message);
                return StatusCode(500, $"Operación inválida: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError("Acceso no autorizado: {Message}", e.Message);
                return StatusCode(401, "No tienes permisos para acceder a este recurso.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error inesperado: {Message}", e.Message);
                return StatusCode(500, $"Ocurrió un error inesperado al intentar eliminar el producto, {e.Message} ");
            }
        }


    }
}
