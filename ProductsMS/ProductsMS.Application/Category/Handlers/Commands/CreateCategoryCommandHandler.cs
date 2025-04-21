using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Category.Commands;

namespace ProductosMs.Application.Category.Handlers.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {

        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler( ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Crear un nuevo ProductId
                var categoryId = CategoryId.Create(Guid.NewGuid());

                // Crear la entidad Producto
                var category = new CategoryEntity(
                    categoryId,
                    CategoryName.Create(request.Category.Name)
                );

                // Guardar el producto en el repositorio
                await _categoryRepository.AddAsync(category);

                // Retornar el ID del producto registrado
                return category.Id.Value;
            }
            catch (Exception ex)
            {
                // Manejo de errores adicional si es necesario
                throw new Exception("An error occurred while registering the category", ex);
            }
        }
    }
}
