using MediatR;
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
            _logger = logger;
            _mediator = mediator;
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("addProduct/{userId}")]
        public async Task<IActionResult> CreatedProduct([FromBody] CreateProductDto createProductDto, [FromRoute] Guid userId)
        {
            try
            {
                var command = new CreateProductCommand(createProductDto, userId);
                var CategoryId = await _mediator.Send(command);
                return Ok(CategoryId);
            }
            catch (ProductNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (ValidatorException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to create an Category");
            }
        }

        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] Guid userId)
        {
            try
            {
                var query = new GetAllProductQuery(userId);
                var Products = await _mediator.Send(query);


                if (Products == null || !Products.Any()) // 🔥 Asegurar que devuelve 404 cuando la lista está vacía
                {
                    return NotFound("No products found");
                }
                return Ok(Products);
            }
            catch (ProductNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to search Product: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to search Product");
            }
        }
       
        
        [HttpGet("available")]
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


                var query = new GetAvailableProductsQuery(userId, categoryId, minPrice, maxPrice);
                var products = await _mediator.Send(query);

                if (products == null || !products.Any())
                {
                    return NotFound("No se encontraron productos disponibles.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAvailableProducts: {ex.Message}");
                return StatusCode(500, "Ocurrió un error inesperado al obtener los productos.");
            }
        }


        [HttpGet("name/product/{name}")]
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



                return Ok(products);
            }
            catch (ProductNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to search Product: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to search Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to search Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An unexpected error occurred: {Message}", e.Message);
                return StatusCode(500, "An unexpected error occurred while trying to search the product.");
            }
        }

        //[Authorize(Policy = "AdminProviderOnly")]
            [HttpGet("{id}")]
            public async Task<IActionResult> GetProduct([FromRoute] Guid id, [FromQuery] Guid userId)
            {
                try
                {
                    var command = new GetProductQuery(id,userId);
                    var Product = await _mediator.Send(command);
                    return Ok(Product);
                }
                catch (CategoryNotFoundException e)
                {
                    _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                    return StatusCode(404, e.Message);
                }
                catch (NullAttributeException e)
                {
                    _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                    return StatusCode(400, e.Message);
                }
                catch (InvalidAttributeException e)
                {
                    _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                    return StatusCode(400, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError("An error occurred while trying to search an Product: {Message}", e.Message);
                    return StatusCode(500, "An error occurred while trying to search an Product");
                }
            }
    
        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto, [FromQuery] Guid userId)
        {
            try
            {
                var command = new UpdateProductCommand(id, updateProductDto, userId);
                var ProductId = await _mediator.Send(command);
                return Ok(ProductId);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to update an Product: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to update an Product");
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, [FromQuery] Guid userId)
        {
            try
            {
                var command = new DeleteProductCommand(id, userId);
                var ProductId = await _mediator.Send(command);
                return Ok(ProductId);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Product: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                //TODO: Colocar validaciones HTTP
                _logger.LogError("An error occurred while trying to delete an Product: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to delete an Product");
            }
        }

    
    }
}
