using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Enum;



namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Crear un nuevo ProductId
                var productId = ProductId.Create(Guid.NewGuid());

                // Validar que la categoría existe
                var category = await _categoryRepository.GetByIdAsync(CategoryId.Create(request.Product.CategoryId));
                if (category == null)
                {
                    throw new Exception("The specified category does not exist.");
                }

                // Crear la entidad Producto
                var product = new ProductEntity(
                    productId,
                    ProductName.Create(request.Product.Name),
                    ProductImage.Create(request.Product.Image),
                    ProductPrice.Create(request.Product.Price),
                    ProductDescription.Create(request.Product.Description),
                    ProductAvilability.Disponible, // Estado inicial
                    ProductStock.Create(request.Product.Stock),
                    category.Id // Usar el ID de la categoría existente
                );

                // Guardar el producto en el repositorio
                await _productRepository.AddAsync(product);

                // Retornar el ID del producto registrado
                return product.Id.Value;
            }
            catch (Exception ex)
            {
                // Manejo de errores adicional si es necesario
                throw new Exception("An error occurred while registering the product", ex);
            }
        }
    }
}
