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
using ProductsMS.Common.Enum;
using AutoMapper;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMs.Domain.Entities.Category.ValueObject;

namespace ProductsMS.Application.Products.Handlers.Queries
{
    public class GetAvailableProductsQueryHandler : IRequestHandler<GetAvailableProductsQuery, List<GetProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetAvailableProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<GetProductDto>> Handle(GetAvailableProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Ejecutando Handle()...");
                Console.WriteLine($"Buscando productos disponibles para el usuario: {request.UserId}");

                var userId = ProductUserId.Create(request.UserId);
                var categoryId = request.CategoryId.HasValue ? CategoryId.Create(request.CategoryId.Value) : null;

                var products = await _productRepository.GetAvailableProductsAsync(
                    userId,
                    categoryId,
                    request.MinPrice,
                    request.MaxPrice
                );

                Console.WriteLine("Consulta realizada correctamente.");

                if (products == null || products.Count == 0)
                {
                    Console.WriteLine("No se encontraron productos disponibles.");
                    return new List<GetProductDto>();
                }

                var productDto = _mapper.Map<List<GetProductDto>>(products);
                return productDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Handle(): {ex.Message}");
                return new List<GetProductDto>();
            }
        }
    }
}
