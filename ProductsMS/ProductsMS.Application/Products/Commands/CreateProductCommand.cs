using MediatR;
using ProductsMS.Common.Dtos.Product.Request;

namespace ProductsMS.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public CreateProductDto Product { get; set; }

        public CreateProductCommand(CreateProductDto product)
        {
            Product = product;
        }
    }
}
