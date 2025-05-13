using MediatR;
using ProductsMs.Core.Repository;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Products.Handlers.Queries
{
   /* public class GetFilteredProductsQueryHandler: IRequestHandler<GetFilteredProductsQuery, List<GetProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetFilteredProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<GetProductDto>> Handle(GetFilteredProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetFilteredProductsAsync(
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.Status
            );

            return products;
        }
    }
   */
}
