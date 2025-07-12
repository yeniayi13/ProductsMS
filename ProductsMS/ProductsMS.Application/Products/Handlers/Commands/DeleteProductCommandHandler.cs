using AutoMapper;
using MediatR;
using ProductsMs.Core.Repository;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Application.Products.Commands;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Common.Exceptions;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Core.Repository;
using ProductsMS.Core.Service.Auction;
using ProductsMS.Core.Service.History;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMS.Infrastructure.Exceptions;

namespace ProductsMS.Application.Products.Handlers.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductRepositoryMongo _productRepositoryMongo;
        private readonly IAuctionService _auctionService;
        private readonly IEventBus<GetProductDto> _eventBus;
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;

        public DeleteProductCommandHandler(IProductRepositoryMongo productRepositoryMongo,IProductRepository productRepository, IEventBus<GetProductDto> eventBus, IMapper mapper, IAuctionService auctionService, IHistoryService historyService)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;
            _mapper = mapper;//*Valido que estas inyecciones sean exitosas
            _productRepositoryMongo = productRepositoryMongo;
            _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService));
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
        }

        public async Task<Guid> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                var productId = ProductId.Create(request.ProductId);
                var userId = ProductUserId.Create(request.UserId);

                var product = await _productRepositoryMongo.GetByIdAsync(productId, userId);
                if (product == null)
                {
                    throw new ProductNotFoundException("Product not found.");
                }
                if (await _auctionService.AuctionExists(productId.Value,userId.Value))
                {
                    throw new ProductInAuctionException("No se puede eliminar un producto mientras está en una subasta ");
                }

                await _productRepository.DeleteAsync(productId);

                var productDto = _mapper.Map<GetProductDto>(product);
                await _eventBus.PublishMessageAsync(productDto, "productQueue", "PRODUCT_DELETED");

                // Registrar la actividad en el historial
                var history = new GetHistoryDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                    Action = $"Eliminastes el Producto con el nombre de :{product.ProductName.Value} ",
                    Timestamp = DateTime.UtcNow
                };
                await _historyService.AddActivityHistoryAsync(history);

                return productId.Value;
            }
            catch (ArgumentNullException ex)
            {
               
                throw;
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (ProductInAuctionException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                
                throw new ApplicationException("Ocurrió un error inesperado al eliminar el producto.", ex);
            }
        }
    }
}
