using AutoMapper;
using MediatR;
using ProductosMs.Application.Products.Queries;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Handlers.Queries
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, GetProductDto>
    {
        public IProductRepository _productRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(IProductRepository productRepository, IApplicationDbContext dbContext, IMapper mapper)
        {
            _productRepository = productRepository;
            _dbContext = dbContext;
            _mapper = mapper; // Inyectar el Mapper
        }

        public async Task<GetProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) throw new NullAttributeException("Product id is required");
            var productId = ProductId.Create(request.Id);
            var product = await _productRepository.GetByIdAsync(productId!, ProductUserId.Create(request.UserId));
            var productDto = _mapper.Map<GetProductDto>(product);
            return productDto;
        }
    }
}
