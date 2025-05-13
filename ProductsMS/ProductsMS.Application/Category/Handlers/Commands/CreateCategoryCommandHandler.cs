using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Category.Commands;
using ProductsMS.Common.Dtos.Category.Request;
using ProductsMS.Core.RabbitMQ;

namespace ProductosMs.Application.Category.Handlers.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IEventBus<CreateCategoryDto> _eventBus;


        public CreateCategoryCommandHandler( ICategoryRepository categoryRepository, IEventBus<CreateCategoryDto> eventBus)
        {
            _categoryRepository = categoryRepository;
            _eventBus = eventBus;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Crear un nuevo ProductId

                //var categoryId = CategoryId.Create(Guid.NewGuid());
                // Crear la entidad Producto

                Console.WriteLine(request.Category.CategoryId);
                Console.WriteLine(request.Category.CategoryName);

                var category = new CategoryEntity(
                    CategoryId.Create(request.Category.CategoryId),
                    CategoryName.Create(request.Category.CategoryName)
                );

               

                // Guardar el producto en el repositorio
                await _categoryRepository.AddAsync(category);
                Console.WriteLine("Hola 3");

                await _eventBus.PublishMessageAsync(request.Category, "productQueue", "CATEGORY_CREATED");
                // Retornar el ID del producto registrado
                return category.CategoryId.Value;
            }
            catch (Exception ex)
            {
                // Manejo de errores adicional si es necesario
                throw new Exception("An error occurred while registering the category", ex);
            }
        }
    }
}
