using MediatR;
using ProductsMS.Common.Dtos.Product.Response;

namespace ProductsMS.Application.Products.Queries
{
    public class GetAllProductQuery:IRequest<List<GetProductDto>>
    {
        public GetAllProductQuery(){ }
    }
}
