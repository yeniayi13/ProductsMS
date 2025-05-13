using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Dtos.Category.Request;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Enum;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Domain.Entities.Products.ValueObjects;



namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEventBus<CreateProductDto> _eventBus;
        public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IEventBus<CreateProductDto> eventBus)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _eventBus = eventBus;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Crear un nuevo ProductId
               // var productId = ProductId.Create(Guid.NewGuid());

                // Validar que la categoría existe
                var category = await _categoryRepository.GetByIdAsync(CategoryId.Create(request.Product.CategoryId));
                if (category == null)
                {
                    throw new Exception("The specified category does not exist.");
                }

                // Crear la entidad Producto
                var product = new ProductEntity(
                    ProductId.Create(request.Product.ProductId),
                    ProductName.Create(request.Product.ProductName),
                    ProductImage.Create(request.Product.ProductImage),
                    ProductPrice.Create(request.Product.ProductPrice),
                    ProductDescription.Create(request.Product.ProductDescription),
                    Enum.Parse<ProductAvilability>(request.Product.ProductAvilability.ToString()!), // Estado inicial
                    ProductStock.Create(request.Product.ProductStock),
                    CategoryId.Create(category.CategoryId.Value) ,// Usar el ID de la categoría existente
                    ProductUserId.Create(request.Product.ProductUserId) // Asignar el ID del usuario
                );

                // Guardar el producto en el repositorio
                await _productRepository.AddAsync(product);
                await _eventBus.PublishMessageAsync(request.Product, "productQueue", "PRODUCT_CREATED");
                // Retornar el ID del producto registrado
                return product.ProductId.Value;
            }
            catch (Exception ex)
            {
                // Manejo de errores adicional si es necesario
                throw new Exception("An error occurred while registering the product", ex);
            }
        }
    }
}
