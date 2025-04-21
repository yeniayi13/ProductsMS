using MediatR;
using ProductsMS.Common.Dtos.Product.Response;

namespace ProductosMs.Application.Products.Queries
{
    public class GetProductQuery : IRequest<GetProductDto>
    {
        public Guid Id { get; set; }

        public GetProductQuery(Guid id)
        {
            Id = id;
        }
    }
}
