using MediatR;
using ProductsMs.Core.Repository;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;

namespace ProductsMS.Application.Products.Handlers.Queries
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<GetProductDto>>
    {
        public IProductRepository _productRepository;

        public GetAllProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<GetProductDto>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAllAsync();

            if (product == null) throw new ProductNotFoundException("Products are empty");

            return product.Where(p => !p.IsDeleted).Select(p =>
                new GetProductDto(
                    p.Id.Value,
                    p.Name.Value,
                    p.Image.Url,
                    p.Price.Value,
                    p.Description.Value,
                    p.Avilability.ToString(),
                    p.Stock.Value,
                    p.CategoryId.Value,
                    p.CreatedBy ?? string.Empty
                )
            ).ToList();
        }
    }
}
