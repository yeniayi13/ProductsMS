using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Test.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;

    public static class DbSetMockExtensions
    {
        public static Mock<DbSet<T>> ToDbSetMock<T>(this IEnumerable<T> source) where T : class
        {
            var queryable = source.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return dbSetMock;
        }
    }
}
