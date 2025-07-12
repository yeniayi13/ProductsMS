

using AutoMapper;
//using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Common.Dtos.Category.Response;


namespace ProductsMs.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        //private readonly IApplicationDbContext _dbContext;
        private readonly IMongoCollection<CategoryEntity> _collection;
        private readonly IMapper _mapper; // Agregar el Mapper


        public CategoryRepository( IMongoCollection<CategoryEntity> collection, IMapper mapper)
        {
            //_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Inyectar el Mapper
        }


       
        public async Task<List<CategoryEntity>> GetAllAsync()
        {
            var projection = Builders<CategoryEntity>.Projection.Exclude("_id").Exclude("CreatedAt").Exclude("CreatedBy");

            // Excluir _id para evitar conflictos con MongoDB

            Console.WriteLine("Consulta de categorías en proceso...");

            var categoryDtos = await _collection
                .Find(Builders<CategoryEntity>.Filter.Empty)
                .Project<GetCategoryDto>(projection) // Obtener el DTO en lugar de la entidad
                .ToListAsync()
                .ConfigureAwait(false);

            if (categoryDtos == null || categoryDtos.Count == 0)
            {
                Console.WriteLine("No se encontraron categorías.");
                return new List<CategoryEntity>(); // Retorna una lista vacía en lugar de `null` para evitar errores
            }

            // Convertir lista de DTOs a lista de entidades usando AutoMapper
            var categoryEntities = _mapper.Map<List<CategoryEntity>>(categoryDtos);

            Console.WriteLine($"Se encontraron {categoryEntities.Count} categorías.");
            return categoryEntities;
        }

        public async Task<CategoryEntity?> GetByIdAsync(CategoryId id)
        {
            Console.WriteLine($"Buscando categoría con ID: {id.Value}");

            var filter = Builders<CategoryEntity>.Filter.Eq("CategoryId", id.Value);
            var projection = Builders<CategoryEntity>.Projection.Exclude("_id").Exclude("CreatedAt").Exclude("CreatedBy").Exclude("UpdateAt").Exclude("UpdateBy");


            var categoryDto = await _collection
                .Find(filter)
                .Project<GetCategoryDto>(projection) // Obtener el DTO en lugar de la entidad
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (categoryDto == null)
            {
                Console.WriteLine("Categoría no encontrada.");
                return null;
            }

            // Convertir DTO a Entidad usando AutoMapper
            var categoryEntity = _mapper.Map<CategoryEntity>(categoryDto);

            Console.WriteLine($"DTO convertido a entidad: {categoryEntity}");
            return categoryEntity;
        }

        public async Task<CategoryEntity?> GetByNameAsync(CategoryName name)
        {
            Console.WriteLine($"Buscando categoría con nombre: {name.Value}");

            var filter = Builders<CategoryEntity>.Filter.Eq("CategoryName", name.Value);
            var projection = Builders<CategoryEntity>.Projection.Exclude("_id").Exclude("CreatedAt")
                .Exclude("CreatedBy").Exclude("UpdateAt").Exclude("UpdateBy");


            var categoryDto = await _collection
                .Find(filter)
                .Project<GetCategoryDto>(projection) // Obtener el DTO en lugar de la entidad
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (categoryDto == null)
            {
                Console.WriteLine("Categoría no encontrada.");
                return null;
            }

            // Convertir DTO a Entidad usando AutoMapper
            var categoryEntity = _mapper.Map<CategoryEntity>(categoryDto);

            Console.WriteLine($"DTO convertido a entidad: {categoryEntity}");
            return categoryEntity;
        }

      /*  public Task<bool> ExistsAsync(CategoryId id)
        {
            return _dbContext.Categories.AnyAsync(x => x.CategoryId == id);
        }*/
    }
}
