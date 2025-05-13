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
        Task<ProductEntity?> GetByIdAsync(ProductId id, ProductUserId userId);
        Task<ProductEntity?> GetByNameAsync(ProductName name, ProductUserId userId);
        Task<List<ProductEntity?>> GetAvailableProductsAsync(ProductUserId userId, CategoryId? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null);

        Task AddAsync(ProductEntity product);
        Task DeleteAsync(ProductId id);
        Task<List<ProductEntity>> GetAllAsync(ProductUserId userId);
        Task<ProductEntity?> UpdateAsync(ProductEntity product);
    }
}
