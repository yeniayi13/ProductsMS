using AutoMapper;
using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Domain.Entities.Products.ValueObjects;

namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventBus<GetProductDto> _eventBus;
        private readonly IMapper _mapper;

        public DeleteProductCommandHandler(IProductRepository productRepository, IEventBus<GetProductDto> eventBus, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;
            _mapper = mapper;//*Valido que estas inyecciones sean exitosas
        }

        public async Task<Guid> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var productId = ProductId.Create(request.ProductId);
                var userId = ProductUserId.Create(request.UserId);
                var products = await _productRepository.GetByIdAsync(productId,userId);
                await _productRepository.DeleteAsync(productId);
                var productDto = _mapper.Map<GetProductDto>(products);
                await _eventBus.PublishMessageAsync(productDto, "productQueue", "PRODUCT_DELETED");
                return productId.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
