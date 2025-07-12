using MediatR;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Enum;


namespace ProductsMS.Application.Products.Queries
{
   

        public class GetFilteredProductsQuery : IRequest<List<GetProductDto>>
        {
            public CategoryId? CategoryId { get; set; }
            public ProductPrice? MinPrice { get; set; }
            public ProductPrice? MaxPrice { get; set; }
            public ProductAvilability Status { get; set; }
        }
    
}
