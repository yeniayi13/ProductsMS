using AutoMapper;
using MediatR;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
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
        private readonly IMapper _mapper; // Agregar el Mapper

        public GetCategoryQueryHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetCategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == Guid.Empty)
                    throw new NullAttributeException("Category id is required");

                var categoryId = CategoryId.Create(request.Id);
                var category = await _categoryRepository.GetByIdAsync(categoryId!);

                if (category == null)
                    throw new CategoryNotFoundException($"Category with ID {request.Id} not found.");

                var categoryDto = _mapper.Map<GetCategoryDto>(category);
                return categoryDto;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCategoryQueryHandler: {ex.Message}");
                throw;
            }
        }
    }
}
