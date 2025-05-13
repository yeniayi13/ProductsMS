using MongoDB.Driver;
using ProductsMs.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Infrastructure.Database.Configuration.Mongo
{
    public class ProductConfigurationMongo
    {
        public static void Configure(IMongoCollection<ProductEntity> collection)
        {
            // Índice único en ProductId para evitar duplicados
            var indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductId.Value);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<ProductEntity>(indexKeysDefinition, indexOptions);
            collection.Indexes.CreateOne(indexModel);

            // Índice en ProductName para optimizar búsqueda por nombre
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductName.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductPrice para consultas de rango de precios
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductPrice.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductAvilability (considerando que es un ENUM)
             indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductAvilability.ToString());
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductStock para optimizar búsquedas por disponibilidad en inventario
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductStock.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en CategoryId para mejorar las relaciones entre categorías y productos
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.CategoryId.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);

            // Índice en ProductUserId para mejorar consultas por usuario propietario
            indexKeysDefinition = Builders<ProductEntity>.IndexKeys.Ascending(p => p.ProductUserId.Value);
            collection.Indexes.CreateOne(indexKeysDefinition);
        }
    }
}
