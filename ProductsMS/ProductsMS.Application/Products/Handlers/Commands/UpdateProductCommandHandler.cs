using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventBus<UpdateProductDto> _eventBus;
        public UpdateProductCommandHandler(IProductRepository productRepository, IEventBus<UpdateProductDto> eventBus)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;

        }

        public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
               Console.WriteLine($"Actualizando producto: {request.Id} para el usuario: {request.UserId}"); // Agregar log para depuración
                var oldProduct = await _productRepository.GetByIdAsync(ProductId.Create(request.Id)!,ProductUserId.Create(request.UserId)!);


                if (oldProduct == null) throw new ProductNotFoundException("Product not found");

                // Validación adicional en `ProductAvailability`
                if (!Enum.TryParse<ProductAvilability>(request.Product.ProductAvilability.ToString(), out var productAvailability))
                {
                    throw new ArgumentException("Invalid ProductAvailability value");
                }

                 //Crear el objeto actualizado con los cambios
                var updatedProduct = ProductEntity.Update(
                    oldProduct, // Se debe proporcionar la entidad base para la actualización
                    request.Product.ProductName != null ? ProductName.Create(request.Product.ProductName) : oldProduct.ProductName,
                    request.Product.ProductImage != null ? ProductImage.Create(request.Product.ProductImage) : oldProduct.ProductImage,
                    request.Product.ProductPrice != null ? ProductPrice.Create(request.Product.ProductPrice) : oldProduct.ProductPrice,
                    request.Product.ProductDescription != null ? ProductDescription.Create(request.Product.ProductDescription) : oldProduct.ProductDescription,
                    productAvailability,
                    request.Product.ProductStock != null ? ProductStock.Create(request.Product.ProductStock) : oldProduct.ProductStock,
                    request.Product.CategoryId != null ? CategoryId.Create(request.Product.CategoryId) : oldProduct.CategoryId,
                    request.Product.ProductUserId != null ? ProductUserId.Create(request.Product.ProductUserId) : oldProduct.ProductUserId

                );

                Console.WriteLine($"Actualizando producto: {oldProduct.ProductUserId}");

                // Actualizar el producto en el repositorio
                await _productRepository.UpdateAsync(oldProduct);
                await _eventBus.PublishMessageAsync(request.Product, "productQueue", "PRODUCT_UPDATED");

                return oldProduct.ProductId.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateProductCommandHandler: {ex.Message}");
                throw;
            }
        }
    }
}
