
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Common.Dtos.Category.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProductsMs.Core.Repository
{
    public interface ICategoryRepository
    {
        Task<CategoryEntity?> GetByIdAsync(CategoryId id/*, Expression<Func<Provider, object>> include*/);

        Task<CategoryEntity?> GetByNameAsync(CategoryName name/*, Expression<Func<Provider, object>> include*/);
        Task AddAsync(CategoryEntity category);
        Task DeleteAsync(CategoryId id);
        Task<List<CategoryEntity>> GetAllAsync();
        Task<CategoryEntity?> UpdateAsync(CategoryEntity category);
    }
}
