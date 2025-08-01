﻿using AutoMapper;
using MongoDB.Driver;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Core.Database;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Core.Repository;

namespace ProductsMS.Infrastructure.Repositories
{
    public class ProductRepositoryMongo : IProductRepositoryMongo
    {
        private readonly IMongoCollection<ProductEntity> _collection;
        private readonly IMapper _mapper; // Agregar el Mapper

        public ProductRepositoryMongo( IMongoCollection<ProductEntity> collection, IMapper mapper)
        {
            
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));// Inyectar el Mapper
        }
       

        public async Task<ProductEntity?> GetByIdAsync(ProductId id, ProductUserId userId)
        {
            Console.WriteLine($"Buscando producto con ID: {id} y usuario: {userId.Value}");

            var filters = Builders<ProductEntity>.Filter.And(
                Builders<ProductEntity>.Filter.Eq("ProductId", id.Value),
                Builders<ProductEntity>.Filter.Eq("ProductUserId", userId.Value) // Filtrar por usuario propietario
            );

            var projection = Builders<ProductEntity>.Projection.Exclude("_id");

            var productDto = await _collection
                .Find(filters)
                .Project<GetProductDto>(projection) // Convertir el resultado al DTO
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (productDto == null)
            {
                Console.WriteLine("Producto no encontrado para este usuario.");
                return null;
            }

            var productEntity = _mapper.Map<ProductEntity>(productDto);
            return productEntity;
        }

        public async Task<ProductEntity?> GetByNameAsync(ProductName name, ProductUserId userId)
        {
            Console.WriteLine($"Buscando producto con nombre: {name} y usuario: {userId.Value}");

            var filters = Builders<ProductEntity>.Filter.And(
                Builders<ProductEntity>.Filter.Eq("ProductName", name.Value),
                Builders<ProductEntity>.Filter.Eq("ProductUserId", userId.Value) // Filtrar por usuario propietario
            );

            var projection = Builders<ProductEntity>.Projection.Exclude("_id");

            var productDto = await _collection
                .Find(filters)
                .Project<GetProductDto>(projection) // Convertir el resultado al DTO
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (productDto == null)
            {
                Console.WriteLine("Producto no encontrado para este usuario.");
                return null;
            }

            var productEntity = _mapper.Map<ProductEntity>(productDto);
            return productEntity;
        }


        public async Task<List<ProductEntity?>> GetAvailableProductsAsync(ProductUserId userId, CategoryId? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            Console.WriteLine("Ejecutando GetAvailableProductsAsync...");

            var filters = new List<FilterDefinition<ProductEntity>>();

            // Filtrar por el usuario propietario del producto
            filters.Add(Builders<ProductEntity>.Filter.Eq("ProductUserId", userId.Value));

            // Filtrar solo productos disponibles
            filters.Add(Builders<ProductEntity>.Filter.Where(p => p.ProductAvilability.ToString() == "Disponible"));

            // Aplicar otros filtros opcionales SOLO si tienen valores válidos
            if (categoryId != null && categoryId.Value != Guid.Empty)
            {
                Console.WriteLine($"Buscando productos de la categoría: {categoryId.Value}");
                filters.Add(Builders<ProductEntity>.Filter.Eq("CategoryId", categoryId.Value));
            }

            if (minPrice.HasValue)
            {
                Console.WriteLine($"Filtrando por precio mínimo: {minPrice.Value}");
                filters.Add(Builders<ProductEntity>.Filter.Gte("ProductPrice", minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                Console.WriteLine($"Filtrando por precio máximo: {maxPrice.Value}");
                filters.Add(Builders<ProductEntity>.Filter.Lte("ProductPrice", maxPrice.Value));
            }

            var filter = Builders<ProductEntity>.Filter.And(filters);

            var projection = Builders<ProductEntity>.Projection.Exclude("_id");

            var productsDto = await _collection
                .Find(filter)
                .Project<GetProductDto>(projection)
                .ToListAsync()
                .ConfigureAwait(false);

            if (productsDto == null || productsDto.Count == 0)
            {
                Console.WriteLine("No se encontraron productos disponibles.");
                return new List<ProductEntity>();
            }

            var productEntities = _mapper.Map<List<ProductEntity>>(productsDto);
            return productEntities;
        }

        public async Task<List<ProductEntity>> GetAllAsync(ProductUserId userId)
        {
            Console.WriteLine($"Consulta de productos en proceso para el usuario: {userId.Value}");

            var filter = Builders<ProductEntity>.Filter.Eq("ProductUserId", userId.Value); // Filtrar por usuario propietario

            var projection = Builders<ProductEntity>.Projection.Exclude("_id");

            var productsDto = await _collection
                .Find(filter) // Aplicamos el filtro
                .Project<GetProductDto>(projection) // Convertir los datos al DTO
                .ToListAsync()
                .ConfigureAwait(false);

            if (productsDto == null || productsDto.Count == 0)
            {
                Console.WriteLine("No se encontraron productos para este usuario.");
                return new List<ProductEntity>(); // Retorna una lista vacía en lugar de `null` para evitar errores
            }

            var productEntities = _mapper.Map<List<ProductEntity>>(productsDto);

            return productEntities;
        }



        /* public Task<bool> ExistsAsync(ProductId id)
         {
             return _dbContext.Products.AnyAsync(x => x.ProductId == id);
         }*/
    }
}
