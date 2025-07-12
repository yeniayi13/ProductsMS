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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var query = new GetAllProductsQuery();
                var categories = await _mediator.Send(query);
                return Ok(categories);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(404, $"Categoría no encontrada: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo nulo detectado: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo inválido detectado: {e.Message}");
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
                _logger.LogError("Error inesperado al buscar categorías: {Message}", e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al intentar buscar categorías.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory([FromRoute] Guid id)
        {
            try
            {
                var command = new GetCategoryQuery(id);
                var category = await _mediator.Send(command);
                return Ok(category);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(404, $"Categoría no encontrada: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo nulo detectado: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo inválido detectado: {e.Message}");
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
                _logger.LogError("Error inesperado al buscar la categoría: {Message}", e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al intentar buscar la categoría.");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryName([FromRoute] string name)
        {
            try
            {
                var command = new GetCategoryNameQuery(name);
                var category = await _mediator.Send(command);
                return Ok(category);
            }
            catch (CategoryNotFoundException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(404, $"Categoría no encontrada: {e.Message}");
            }
            catch (NullAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo nulo detectado: {e.Message}");
            }
            catch (InvalidAttributeException e)
            {
                _logger.LogError("Error: {Message}", e.Message);
                return StatusCode(400, $"Atributo inválido detectado: {e.Message}");
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
                _logger.LogError("Error inesperado al buscar la categoría por nombre: {Message}", e.Message);
                return StatusCode(500, "Ocurrió un error inesperado al intentar buscar la categoría.");
            }
        }

    }
}
