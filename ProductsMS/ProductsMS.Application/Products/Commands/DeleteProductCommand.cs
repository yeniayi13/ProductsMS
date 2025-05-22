using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Products.Commands
{
    public class DeleteProductCommand : IRequest<Guid>
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        public DeleteProductCommand(Guid productId, Guid userId)
        {
            ProductId = productId;
            UserId = userId;
        }
    }
}
