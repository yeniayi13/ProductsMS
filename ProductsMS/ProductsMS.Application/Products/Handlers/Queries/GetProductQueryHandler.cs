using MediatR;
using ProductosMs.Application.Products.Queries;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;

namespace ProductsMS.Application.Products.Handlers.Queries
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, GetProductDto>
    {
        public IProductRepository _productRepository;
        private readonly IApplicationDbContext _dbContext;

        public GetProductQueryHandler(IProductRepository productRepository, IApplicationDbContext dbContext)
        {
            _productRepository = productRepository;
            _dbContext = dbContext;
        }

        public async Task<GetProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) throw new NullAttributeException("Product id is required");
            var productId = ProductId.Create(request.Id);
            var product = await _productRepository.GetByIdAsync(productId!);
            var createdBy = product.CreatedBy ?? string.Empty;

            return new GetProductDto(
                product.Id.Value,
                product.Name.Value,
                product.Image.Url,
                product.Price.Value,
                product.Description.Value,
                product.Avilability.ToString(),
                product.Stock.Value,
                product.CategoryId.Value,
                createdBy
            );
        }
    }
}
