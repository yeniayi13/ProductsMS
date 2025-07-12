using MediatR;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Queries
{
    public class GetAllProductQuery:IRequest<List<GetProductDto>>
    {

        public Guid UserId { get; set; }
        public GetAllProductQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
