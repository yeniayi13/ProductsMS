using MediatR;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Enum;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Products.Queries
{
    public class GetAvailableProductsQuery : IRequest<List<GetProductDto>>
    {
        public Guid UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public GetAvailableProductsQuery(Guid userId, Guid? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            UserId = userId;
            CategoryId = categoryId;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }
    }
}
