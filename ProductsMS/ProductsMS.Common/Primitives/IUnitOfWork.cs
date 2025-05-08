using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsMS.Common.Primitives
{
    public interface IUnitOfWork
    {
        //TODO Borrar UnitOfWork
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}