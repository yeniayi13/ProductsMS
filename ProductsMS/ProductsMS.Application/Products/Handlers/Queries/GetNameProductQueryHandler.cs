using MediatR;
using ProductosMs.Application.Products.Queries;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Application.Products.Queries;
using ProductsMS.Common.Dtos.Product.Response;
using AutoMapper;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Handlers.Queries
{
    public class GetNameProductQueryHandler : IRequestHandler<GetNameProductQuery, GetProductDto>
    {
        public IProductRepository _productRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper; // Agregar el Mapper
        public GetNameProductQueryHandler(IProductRepository productRepository, IApplicationDbContext dbContext, IMapper mapper)
        {
            _productRepository = productRepository;
            _dbContext = dbContext;
            _mapper = mapper;// Inyectar el Mapper
        }

        public async Task<GetProductDto> Handle(GetNameProductQuery request, CancellationToken cancellationToken)
        {
            //if (request.Id == Guid.Empty) throw new NullAttributeException("Product id is required");
            var productName = ProductName.Create(request.Name);
            var userId = ProductUserId.Create(request.UserId);
            var product = await _productRepository.GetByNameAsync(productName!,userId!);
            var productDto = _mapper.Map<GetProductDto>(product);
            return productDto;
        }
    }
}
