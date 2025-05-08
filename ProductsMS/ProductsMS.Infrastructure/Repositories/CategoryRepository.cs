

using Microsoft.EntityFrameworkCore;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;


namespace ProductsMs.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public CategoryRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task AddAsync(CategoryEntity category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<CategoryEntity?> GetByIdAsync(CategoryId id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            //TODO: Borrar todos los console
            return category;
        }

        public async Task<CategoryEntity?> GetByNameAsync(CategoryName name)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == name);
            //TODO: Borrar todos los console
            return category;
        }

        public async Task<List<CategoryEntity>?> GetAllAsync()
        {
            var departments = await _dbContext.Categories.ToListAsync();
            return departments;
        }

        public async Task DeleteAsync(CategoryId id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            //if (department == null) throw new DepartmentNotFoundException("department not found");
            _dbContext.Categories.Remove(category);
            //department.IsDeleted = true;
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<CategoryEntity?> UpdateAsync(CategoryEntity category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveEfContextChanges("");
            return category;
        }
        public Task<bool> ExistsAsync(CategoryId id)
        {
            return _dbContext.Categories.AnyAsync(x => x.Id == id);
        }
    }
}
