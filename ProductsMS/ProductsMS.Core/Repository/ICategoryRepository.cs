
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
        Task<CategoryEntity?> GetByIdAsync(CategoryId id);

        Task<CategoryEntity?> GetByNameAsync(CategoryName name);
       
        Task<List<CategoryEntity>> GetAllAsync();
    }
}
