using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductsMs.Core.Database;
using System.Diagnostics.CodeAnalysis;

namespace ProductosMs.Infrastructure.Database
{


    [ExcludeFromCodeCoverage]
    public class DbContextTransactionProxy : IDbContextTransactionProxy
    {

        private readonly IDbContextTransaction _transaction;

        private bool _disposed;

        public DbContextTransactionProxy(DbContext context)
        {
            _transaction = context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction.Dispose();
                }

                _disposed = true;
            }
        }

    }
}