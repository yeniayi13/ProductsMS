using MediatR;
using ProductsMS.Common.Dtos.Category.Response;

namespace ProductsMS.Application.Category.Queries
{
    public class GetAllProductsQuery : IRequest<List<GetCategoryDto>>
    {
        public GetAllProductsQuery() { }
    }
}
