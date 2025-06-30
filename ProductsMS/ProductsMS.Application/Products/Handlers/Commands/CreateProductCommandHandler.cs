using AutoMapper;
using FluentValidation;
using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Application.Products.Validator.Products;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Core.Service.History;
using ProductsMS.Core.Service.User;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMS.Infrastructure.Exceptions;


namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEventBus<GetProductDto> _eventBus;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        public CreateProductCommandHandler(IMapper mapper,IUserService userService,IProductRepository productRepository, ICategoryRepository categoryRepository, IEventBus<GetProductDto> eventBus,IHistoryService historyService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _eventBus = eventBus;
            _userService = userService;
            _mapper = mapper;
            _historyService = historyService;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Valido los datos de entrada
                var validator = new CreateProductEntityValidator();
                var validationResult = await validator.ValidateAsync(request.Product, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors); // No lo capturamos en un Exception genérico
                }

                // Validar que la categoría existe
                var category = await _categoryRepository.GetByIdAsync(CategoryId.Create(request.Product.CategoryId));
                if (category == null)
                {
                    throw new CategoryNotFoundException("The specified category does not exist.");
                }

                var user = await _userService.AuctioneerExists(request.UserId);

                
                if (user == null) throw new UserNotFoundException($"user with id {request.UserId} not found");

                // Crear la entidad Producto
                var product = new ProductEntity(
                    ProductId.Create(request.Product.ProductId),
                    ProductName.Create(request.Product.ProductName),
                    ProductImage.FromBase64(request.Product.ProductImage),
                    ProductPrice.Create(request.Product.ProductPrice),
                    ProductDescription.Create(request.Product.ProductDescription),
                    Enum.Parse<ProductAvilability>(request.Product.ProductAvilability.ToString()!), // Estado inicial
                    ProductStock.Create(request.Product.ProductStock),
                    CategoryId.Create(category.CategoryId.Value), // Usar el ID de la categoría existente
                    ProductUserId.Create(user.UserId) // Asignar el ID del usuario
                );

                var productDto = _mapper.Map<GetProductDto>(product);

                // Guardar el producto en el repositorio
                await _productRepository.AddAsync(product);
                await _eventBus.PublishMessageAsync(productDto, "productQueue", "PRODUCT_CREATED");

                // Registrar la actividad en el historial
                var history = new GetHistoryDto
                {
                    Id = Guid.NewGuid(),
                    UserId = user.UserId,
                    Action = $"Creaste el Producto con el nombre de :{product.ProductName.Value} ",
                    Timestamp = DateTime.UtcNow
                };
                await _historyService.AddActivityHistoryAsync(history);

                // Retornar el ID del producto registrado
                return product.ProductId.Value;
            }
            catch (ValidationException ex)
            {
                throw;

            }
            catch (UserNotFoundException ex)
            {
                throw;

            }
            catch (CategoryNotFoundException ex)
            {
                throw;

            }
            catch (Exception ex)
            {
                throw;

            }
        }
    }
    
}
