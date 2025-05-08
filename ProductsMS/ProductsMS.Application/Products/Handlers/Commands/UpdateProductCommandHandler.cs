using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Exceptions;

namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository)); //*Valido que estas inyecciones sean exitosas


        }

        public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var oldProduct = await _productRepository.GetByIdAsync(ProductId.Create(request.Id)!);

                if (oldProduct == null) throw new ProductNotFoundException("provider not found");

                if ((request.Product.Name != null) && (request.Product.Avilability == 1))
                {
                    oldProduct = ProductEntity.Update(oldProduct, ProductName.Create(request.Product.Name), null, null, null, ProductAvilability.NoDisponible, null);
                }
                else if ((request.Product.Name != null) && (request.Product.Avilability == 0))
                {
                    oldProduct = ProductEntity.Update(oldProduct, ProductName.Create(request.Product.Name), null, null, null, ProductAvilability.Disponible, null);
                }
                if ((request.Product.Image != null)&& (request.Product.Avilability == 1))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, ProductImage.Create(request.Product.Image), null, null,  ProductAvilability.NoDisponible, null);
                }
                else if ((request.Product.Image != null) && (request.Product.Avilability == 0))
                {
                    oldProduct = ProductEntity.Update(oldProduct, ProductName.Create(request.Product.Name), null, null, null, ProductAvilability.Disponible, null);
                }
                if ((request.Product.Price != null)&& (request.Product.Avilability == 1))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, null, ProductPrice.Create(request.Product.Price), null, ProductAvilability.NoDisponible, null);
                }
                else if ((request.Product.Price != null) && (request.Product.Avilability == 0))
                {
                    oldProduct = ProductEntity.Update(oldProduct, ProductName.Create(request.Product.Name), null, null, null, ProductAvilability.Disponible, null);
                }
                if ((request.Product.Description != null)&& (request.Product.Avilability == 1))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, null, null, ProductDescription.Create(request.Product.Description),ProductAvilability.NoDisponible, null);
                }
                else if ((request.Product.Description != null) && (request.Product.Avilability == 0))
                {
                    oldProduct = ProductEntity.Update(oldProduct, ProductName.Create(request.Product.Name), null, null, null, ProductAvilability.Disponible, null);
                }
                if ((request.Product.Avilability != null) &&( request.Product.Avilability == 1))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, null, null, null, ProductAvilability.Disponible, null);
                }
                else if ((request.Product.Avilability != null) && (request.Product.Avilability == 0))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, null, null, null, ProductAvilability.NoDisponible, null);
                }
                if ((request.Product.Stock != null) && (request.Product.Avilability == 1))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, null, null, null, ProductAvilability.NoDisponible, ProductStock.Create(request.Product.Stock));
                }
                else if ((request.Product.Stock != null) && (request.Product.Avilability == 0))
                {
                    oldProduct = ProductEntity.Update(oldProduct, null, null, null, null, ProductAvilability.Disponible, null);
                }

                await _productRepository.UpdateAsync(oldProduct);

                return oldProduct.Id.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
