using MediatR;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductosMs.Application.Products.Queries
{
    public class GetProductQuery : IRequest<GetProductDto>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public GetProductQuery(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}
