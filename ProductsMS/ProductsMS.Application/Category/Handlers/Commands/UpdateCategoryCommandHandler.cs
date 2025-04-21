using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Category.Commands;
using ProductsMS.Common.Exceptions;

namespace ProductosMs.Application.Category.Handlers.Commands
{
     public class UpdateCategoryCommandHandler:IRequestHandler<UpdateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categoryRepository;
        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository)); //*Valido que estas inyecciones sean exitosas


        }

        public async Task<Guid> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var oldCategory = await _categoryRepository.GetByIdAsync(CategoryId.Create(request.CategoryId)!);

                if (oldCategory == null) throw new CategoryNotFoundException("category not found");

                if ((request.Category.Name != null) )
                {
                    oldCategory = CategoryEntity.Update(oldCategory, CategoryName.Create(request.Category.Name));
                }
             
                await _categoryRepository.UpdateAsync(oldCategory);

                return oldCategory.Id.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateCategoryCommandHandler: {ex.Message}");
                throw;
            }
        }
    }
}
