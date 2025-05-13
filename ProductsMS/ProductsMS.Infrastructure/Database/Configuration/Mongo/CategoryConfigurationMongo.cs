using MongoDB.Driver;
using ProductsMs.Domain.Entities.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Infrastructure.Database.Configuration.Mongo
{
    public class CategoryConfigurationMongo
    {
        public static void Configure(IMongoCollection<CategoryEntity> collection)
        {
            // Crear un índice único en CategoryId para evitar duplicados
            var indexKeysDefinition = Builders<CategoryEntity>.IndexKeys.Ascending(c => c.CategoryId.Value);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<CategoryEntity>(indexKeysDefinition, indexOptions);
            collection.Indexes.CreateOne(indexModel);

            // Índice en CategoryName para mejorar la búsqueda por nombre
            indexKeysDefinition = Builders<CategoryEntity>.IndexKeys.Ascending(c => c.CategoryName.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en Products (puede requerir consideración adicional si se usa en consultas)
            indexKeysDefinition = Builders<CategoryEntity>.IndexKeys.Ascending(c => c.Products.Count);
            collection.Indexes.CreateOne(indexKeysDefinition);
        }
    }
}
