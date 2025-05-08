using Microsoft.EntityFrameworkCore;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Enum;

namespace ProductsMs.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public ProductRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task AddAsync(ProductEntity product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<ProductEntity?> GetByIdAsync(ProductId id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            //TODO: Borrar todos los console
            return product;
        }

        public async Task<ProductEntity?> GetByNameAsync(ProductName name)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Name == name);
            //TODO: Borrar todos los console
            return product;
        }

        public async Task<List<ProductEntity>?> GetAllAsync()
        {
            var product = await _dbContext.Products.ToListAsync();
            return product;
        }

        public async Task DeleteAsync(ProductId id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            //if (department == null) throw new DepartmentNotFoundException("department not found");
            _dbContext.Products.Remove(product);
            //department.IsDeleted = true;
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<List<ProductEntity>> GetFilteredProductsAsync(CategoryId? categoryId, ProductPrice? minPrice, ProductPrice? maxPrice, ProductAvilability? status)
        {
            var query = _dbContext.Products.AsQueryable();

            // Filtrar por CategoryId
            if (categoryId != null)
            {
                query = query.Where(p => p.CategoryId.Value == categoryId.Value);
            }

            // Filtrar por precio mínimo
            if (minPrice != null)
            {
                query = query.Where(p => p.Price.Value >= minPrice.Value);
            }

            // Filtrar por precio máximo
            if (maxPrice != null)
            {
                query = query.Where(p => p.Price.Value <= maxPrice.Value);
            }

            // Filtrar por estado (Avability)
            if (status.HasValue)
            {
                query = query.Where(p => p.Avilability == status.Value);
            }

            return await query.ToListAsync();
        }


        public async Task<ProductEntity?> UpdateAsync(ProductEntity product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveEfContextChanges("");
            return product;
        }
        public Task<bool> ExistsAsync(ProductId id)
        {
            return _dbContext.Products.AnyAsync(x => x.Id == id);
        }
    }
}
