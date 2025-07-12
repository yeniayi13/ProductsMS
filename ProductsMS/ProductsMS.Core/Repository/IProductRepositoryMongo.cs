using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Core.Repository
{
    public interface IProductRepositoryMongo
    {
        Task<ProductEntity?> GetByIdAsync(ProductId id, ProductUserId userId);
        Task<ProductEntity?> GetByNameAsync(ProductName name, ProductUserId userId);
        Task<List<ProductEntity?>> GetAvailableProductsAsync(ProductUserId userId, CategoryId? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<List<ProductEntity>> GetAllAsync(ProductUserId userId);
    }
}
