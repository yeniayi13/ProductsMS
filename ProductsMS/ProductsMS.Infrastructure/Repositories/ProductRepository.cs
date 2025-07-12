using AutoMapper;
//using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Enum;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMs.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper; // Agregar el Mapper

        public ProductRepository(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
          
        }
        public async Task AddAsync(ProductEntity product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveEfContextChanges("");
        }


        public async Task DeleteAsync(ProductId id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            //if (department == null) throw new DepartmentNotFoundException("department not found");
            _dbContext.Products.Remove(product);
            //department.IsDeleted = true;
            await _dbContext.SaveEfContextChanges("");
        }

        public async Task<ProductEntity?> UpdateAsync(ProductEntity product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveEfContextChanges("");
            return product;
        }
     
    }
}
