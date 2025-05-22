using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using ProductsMS.Infrastructure.Services.Auction;
using ProductsMs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProductsMS.Test.Service
{
    public class AuctionServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IOptions<HttpClientUrl>> _mockHttpClientUrl;
        private readonly AuctionService _auctionService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public AuctionServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpClientUrl = new Mock<IOptions<HttpClientUrl>>();

            _mockHttpClientUrl.Setup(x => x.Value).Returns(new HttpClientUrl { ApiUrl = "https://example.com" });

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["Authorization"] = "Bearer test-token";
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            // Simulación de HttpClient usando HttpMessageHandler
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com")
            };

            _auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);
        }

        [Fact]
        public async Task AuctionExists_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Valid response")
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act
            var result = await auctionService.AuctionExists(productId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowHttpRequestException_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(productId));
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowInvalidOperationException_WhenResponseContentIsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty) // Simula una respuesta vacía en lugar de `null`
            };


            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => auctionService.AuctionExists(productId));
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowHttpRequestException_WhenServerFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(productId));
        }
    }
}
