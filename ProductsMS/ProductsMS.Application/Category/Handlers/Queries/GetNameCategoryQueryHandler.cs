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
using MediatR;

namespace ProductsMS.Application.Category.Handlers.Queries
{
    public class GetNameCategoryQueryHandler: IRequestHandler<GetCategoryNameQuery, CategoryEntity>
    {
        public ICategoryRepository _categoryRepository;
        private readonly IApplicationDbContext _dbContext;

    public GetNameCategoryQueryHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext)
    {
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
    }

    public async Task<CategoryEntity> Handle(GetCategoryNameQuery request, CancellationToken cancellationToken)
    {
        if (request.Name == String.Empty) throw new NullAttributeException("Category name is required");
        var categoryId = CategoryName.Create(request.Name);
        var category = await _categoryRepository.GetByNameAsync(categoryId!);
        //var createdBy = category.CreatedBy ?? string.Empty;

        return category;
    }
}
}
