using MediatR;
using ProductsMS.Common.Dtos.Product.Request;

namespace ProductsMS.Application.Products.Commands
{
    public class UpdateProductCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public UpdateProductDto Product;

        public UpdateProductCommand(Guid id, UpdateProductDto product)
        {
            Id = id;
            Product = product;
        }
    }
}
