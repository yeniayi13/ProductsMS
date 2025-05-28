using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Enum;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMs.Core.Repository
{
    public interface IProductRepository
    {
        Task AddAsync(ProductEntity product);
        Task DeleteAsync(ProductId id);
        Task<ProductEntity?> UpdateAsync(ProductEntity product);
    }
}
