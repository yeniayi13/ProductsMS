using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductosMs.Application.Products.Queries;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;
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
        [HttpPost]
        public async Task<IActionResult> CreatedCategory([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                var command = new CreateProductCommand(createProductDto);
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
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var query = new GetAllProductQuery();
                var Products = await _mediator.Send(query);
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

        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id)
        {
            try
            {
                var command = new GetProductQuery(id);
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
        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredProducts(
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] ProductAvilability? status,
            [FromQuery] bool? isAvailable)
        {
            var query = new GetFilteredProductsQuery
            {
                CategoryId = categoryId.HasValue ? CategoryId.Create(categoryId.Value) : null,
                //MinPrice = minPrice.HasValue ? ProductPrice.Create(ProductPrice.Value) : null,
                //MaxPrice = maxPrice,
                //Status = status,
                
            };

            var products = await _mediator.Send(query);
            return Ok(products);
        }
        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                var command = new UpdateProductCommand(id, updateProductDto);
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
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            try
            {
                var command = new DeleteProductCommand(id);
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
