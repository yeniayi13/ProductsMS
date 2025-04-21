using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Category.Commands;


namespace ProductsMS.Application.Category.Handlers.Commands
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _categoryRepository;
        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository)); //*Valido que estas inyecciones sean exitosas

        }

        public async Task<Guid> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryId = CategoryId.Create(request.CategoryId);
                await _categoryRepository.DeleteAsync(categoryId);
                return categoryId.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
