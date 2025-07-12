using MediatR;
using ProductsMs.Domain.Entities.Category;
using ProductsMS.Common.Dtos.Category.Response;

namespace ProductsMS.Application.Category.Queries
{
    public class GetAllProductsQuery : IRequest<List<GetCategoryDto>>
    {
        public GetAllProductsQuery() { }
    }
}
