using Microsoft.EntityFrameworkCore;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Test.DataSeed
{
    public class TestDbContextCategory : DbContext
    {
        public DbSet<CategoryEntity> Categories { get; set; }

        public TestDbContextCategory(DbContextOptions<TestDbContextCategory> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryEntity>().HasData(
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Electrónicos")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Ropa")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Hogar y Decoración")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Deportes y Fitness")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Alimentos y Bebidas")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Automóviles y Motos")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Libros y Revistas")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Juguetes y Videojuegos")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Salud y Belleza")
                ),
                new CategoryEntity
                (
                    CategoryId.Create(Guid.NewGuid()),
                    CategoryName.Create("Tecnología y Gadgets")
                )
            );
        }
    }
}