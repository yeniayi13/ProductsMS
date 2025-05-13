using AutoMapper;
using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Handlers.Queries
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<GetProductDto>>
    {
        public IProductRepository _productRepository;
        private readonly IMapper _mapper; // Agregar el Mapper

        public GetAllProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Inyectar el Mapper
        }

        public async Task<List<GetProductDto>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAllAsync(ProductUserId.Create(request.UserId));

            if (product == null) throw new ProductNotFoundException("Products are empty");
            var productDto = _mapper.Map<List<GetProductDto>>(product);

            return productDto;
        }
    }
}
