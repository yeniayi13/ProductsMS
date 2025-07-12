using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Test.DataSeed
{
    public static class MongoDbSeeder
    {
        public static async Task SeedCategoriesAsync(IMongoCollection<CategoryEntity> collection)
        {
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Electrónicos")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Ropa")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Hogar y Decoración")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Deportes y Fitness")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Alimentos y Bebidas")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Automóviles y Motos")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Libros y Revistas")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Juguetes y Videojuegos")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Salud y Belleza")),
                new CategoryEntity(CategoryId.Create(Guid.NewGuid()), CategoryName.Create("Tecnología y Gadgets"))
            };

            var existingCount = await collection.CountDocumentsAsync(FilterDefinition<CategoryEntity>.Empty);
            if (existingCount == 0) // Evitar duplicados en cada prueba
            {
                await collection.InsertManyAsync(categories);
                Console.WriteLine("✅ Datos de prueba insertados en MongoDB.");
            }
        }
    }
}
