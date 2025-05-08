using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Enum;

namespace ProductsMs.Core.Repository
{
    public interface IProductRepository
    {
        Task<ProductEntity?> GetByIdAsync(ProductId id/*, Expression<Func<Provider, object>> include*/);
        Task<List<ProductEntity>> GetFilteredProductsAsync(CategoryId? categoryId, ProductPrice? minPrice, ProductPrice? maxPrice, ProductAvilability? status);
        Task<ProductEntity?> GetByNameAsync(ProductName name/*, Expression<Func<Provider, object>> include*/);
        Task AddAsync(ProductEntity product);
        Task DeleteAsync(ProductId id);
        Task<List<ProductEntity>> GetAllAsync();
        Task<ProductEntity?> UpdateAsync(ProductEntity product);
    }
}
