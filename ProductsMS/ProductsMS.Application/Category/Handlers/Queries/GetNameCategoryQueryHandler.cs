using Microsoft.EntityFrameworkCore;
using ProductsMS.Application.Category.Queries;
using ProductsMS.Common.Exceptions;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using ProductsMS.Common.Dtos.Category.Response;

namespace ProductsMS.Application.Category.Handlers.Queries
{
    public class GetNameCategoryQueryHandler: IRequestHandler<GetCategoryNameQuery, GetCategoryDto>
    {
        public ICategoryRepository _categoryRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper; // Agregar el Mapper

        public GetNameCategoryQueryHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
        _mapper = mapper;
        }

    public async Task<GetCategoryDto> Handle(GetCategoryNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new NullAttributeException("Category name is required");

            var categoryId = CategoryName.Create(request.Name);
            var category = await _categoryRepository.GetByNameAsync(categoryId!);

            if (category == null)
                throw new CategoryNotFoundException($"Category with name '{request.Name}' not found.");

            var categoryDto = _mapper.Map<GetCategoryDto>(category);
            return categoryDto;
}
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCategoryNameQueryHandler: {ex.Message}");
            throw;
        }
    }
 }
}
