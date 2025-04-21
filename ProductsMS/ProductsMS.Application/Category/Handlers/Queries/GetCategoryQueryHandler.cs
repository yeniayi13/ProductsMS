using MediatR;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMS.Common.Exceptions;


namespace ProductosMs.Application.Category.Handlers.Queries
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, GetCategoryDto>
    {
        public ICategoryRepository _categoryRepository;
        private readonly IApplicationDbContext _dbContext;

        public GetCategoryQueryHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext)
        {
            _categoryRepository = categoryRepository;
            _dbContext = dbContext;
        }

        public async Task<GetCategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) throw new NullAttributeException("Category id is required");
            var categoryId = CategoryId.Create(request.Id);
            var category = await _categoryRepository.GetByIdAsync(categoryId!);
            var createdBy = category.CreatedBy ?? string.Empty;

            return new GetCategoryDto(
                category.Id.Value,
                category.Name.Value,
                createdBy
            );
        }
    }
}
