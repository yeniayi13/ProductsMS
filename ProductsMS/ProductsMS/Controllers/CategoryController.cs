using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Exceptions;
using ProductsMS.Infrastructure.Exceptions;

namespace ProductosMs.Controllers
{
    [ApiController]
    [Route("auctioneer/category")]
    public class CategoryController:ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IMediator _mediator;

        public CategoryController(ILogger<CategoryController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var query = new GetAllProductsQuery();
                var Categories = await _mediator.Send(query);
                return Ok(Categories);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to search Category: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to search Category");
            }
        }

        //[Authorize(Policy = "AdminProviderOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory([FromRoute] Guid id)
        {
            try
            {
                var command = new GetCategoryQuery(id);
                var Category = await _mediator.Send(command);
                return Ok(Category);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to search an Category: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to search an Category");
            }
        }


        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryName([FromRoute] string name)
        {
            try
            {
                var command = new GetCategoryNameQuery(name);
                var Category = await _mediator.Send(command);
                return Ok(Category);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(404, e.Message);
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("An error occurred while trying to create an Category: {Message}", e.Message);
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while trying to search an Category: {Message}", e.Message);
                return StatusCode(500, "An error occurred while trying to search an Category");
            }
        }

    }
}
