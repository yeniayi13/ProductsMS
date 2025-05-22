using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductosMs.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ProductsMS.Infrastructure.Database.Context.Postgres;
using ProductsMS.Infrastructure.Database.Factory.Postgres;

namespace ProductsMS.Test.Infrastructure.DataBase
{
    public class ApplicationDbContextFactoryTests
    {
        [Fact]
        public void CreateDbContext_ShouldReturnValidDbContext()
        {
            // Arrange
            var factory = new ApplicationDbContextFactory();

            // Act
            var dbContext = factory.CreateDbContext(new string[] { });

            // Assert
            Assert.NotNull(dbContext);
            Assert.IsType<ApplicationDbContext>(dbContext);
        }

        [Fact]
        public void CreateDbContext_ShouldUsePostgreSQLConfiguration()
        {
            // Arrange
            var factory = new ApplicationDbContextFactory();

            // Act
            var dbContext = factory.CreateDbContext(new string[] { });

            // Assert
            Assert.Contains("Npgsql", dbContext.Database.ProviderName); // 🔥 Confirma que está usando PostgreSQL
        }
    }

}
