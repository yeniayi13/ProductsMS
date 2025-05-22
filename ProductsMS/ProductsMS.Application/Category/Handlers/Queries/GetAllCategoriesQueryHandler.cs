using AutoMapper;
using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;


namespace ProductosMs.Application.Category.Handlers.Queries
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllProductsQuery, List<GetCategoryDto>>
    {
        public ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper; // Agregar el Mapper

        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;

        }

        public async Task<List<GetCategoryDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAllAsync();

            if (category == null) throw new CategoryNotFoundException("Categories are empty");
            var categoryDto = _mapper.Map<List<GetCategoryDto>>(category);
            return categoryDto;
        }
    }
}
