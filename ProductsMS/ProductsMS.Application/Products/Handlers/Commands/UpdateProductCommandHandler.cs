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
using ProductsMS.Core.Repository;
using ProductsMS.Core.Service.History;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductRepositoryMongo _productRepositoryMongo;
        private readonly IEventBus<UpdateProductDto> _eventBus;
        private readonly IHistoryService _historyService;
        public UpdateProductCommandHandler(IProductRepository productRepository, IEventBus<UpdateProductDto> eventBus, IProductRepositoryMongo productRepositoryMongo, IHistoryService historyService)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;
            _productRepositoryMongo = productRepositoryMongo ?? throw new ArgumentNullException(nameof(productRepositoryMongo));
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));

        }

        public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                if (request.Product == null)
                {
                    throw new ArgumentNullException(nameof(request.Product), "Product cannot be null.");
                }

                var oldProduct = await _productRepositoryMongo.GetByIdAsync(ProductId.Create(request.Id)!, ProductUserId.Create(request.UserId)!);

                // Valido los datos de entrada
                var validator = new UpdateProductEntityValidator();
                var validationResult = await validator.ValidateAsync(request.Product, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new FluentValidation.ValidationException(validationResult.Errors);
                }

                if (oldProduct == null)
                {
                    throw new ProductNotFoundException("Product not found");
                }

                // Validación adicional en `ProductAvailability`
                if (!Enum.TryParse<ProductAvilability>(request.Product.ProductAvilability.ToString(), out var productAvailability))
                {
                    throw new ArgumentException("Invalid ProductAvailability value");
                }

                // Crear el objeto actualizado con los cambios
                var updatedProduct = ProductEntity.Update(
                    oldProduct,
                    request.Product.ProductName != null ? ProductName.Create(request.Product.ProductName) : oldProduct.ProductName,
                    request.Product.ProductImage != null ? ProductImage.FromBase64(request.Product.ProductImage) : oldProduct.ProductImage,
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

                // Registrar la actividad en el historial
                var history = new GetHistoryDto
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Action = $"Actualizaste el Producto con el nombre de :{oldProduct.ProductName.Value} ",
                    Timestamp = DateTime.UtcNow
                };
                await _historyService.AddActivityHistoryAsync(history);

                return oldProduct.ProductId.Value;
            }
            catch (ArgumentNullException ex)
            {
              
                throw;
            }
            catch (FluentValidation.ValidationException ex)
            {
                
                throw;
            }
            catch (ProductNotFoundException ex)
            {
                
                throw;
            }
            catch (ArgumentException ex)
            {
                
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error inesperado al actualizar el producto.", ex);
            }
        }
    }
    
}
