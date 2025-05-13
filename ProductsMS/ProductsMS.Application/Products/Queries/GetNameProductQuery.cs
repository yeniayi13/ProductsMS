using MediatR;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Products.Queries
{
    public class GetNameProductQuery : IRequest<GetProductDto>
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public GetNameProductQuery(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
        }
    }
}
