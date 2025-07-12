using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsMs.Core.Database;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Products;


namespace ProductsMs.Core.Database
{
    public interface IApplicationDbContext
    {
        DbContext DbContext { get; }
        DbSet<ProductEntity> Products { get; set; }
        DbSet<CategoryEntity> Categories { get; set; }
       

        IDbContextTransactionProxy BeginTransaction();

        void ChangeEntityState<TEntity>(TEntity entity, EntityState state);

        Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default);
    }
}