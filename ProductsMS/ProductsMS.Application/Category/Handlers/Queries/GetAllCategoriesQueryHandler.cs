using MediatR;
using ProductsMs.Core.Repository;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Dtos.Category.Response;


namespace ProductosMs.Application.Category.Handlers.Queries
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllProductsQuery, List<GetCategoryDto>>
    {
        public ICategoryRepository _categoryRepository;

        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<GetCategoryDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAllAsync();

            //if (provider == null) throw new ProviderNotFoundException("Providers are empty");

            return category.Where(c => !c.IsDeleted).Select(c =>
                new GetCategoryDto(
                    c.Id.Value,
                    c.Name.Value,
                    c.CreatedBy ?? string.Empty
                )
            ).ToList();
        }
    }
}
