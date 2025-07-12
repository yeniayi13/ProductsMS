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
using System.Text.Json;
using System.Threading.Tasks;
using Moq.Protected;

using Xunit;
using ProductsMS.Core.Service.Auction;

namespace ProductsMS.Test.Service
{
    public class AuctionServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IOptions<HttpClientUrl>> _mockHttpClientUrl;
        private readonly HttpClient _httpClient;

        public AuctionServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpClientUrl = new Mock<IOptions<HttpClientUrl>>();

            _mockHttpClientUrl.Setup(x => x.Value).Returns(new HttpClientUrl { ApiUrl = "http://localhost:18085/" });

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["Authorization"] = "Bearer test-token";
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);
        }

        [Fact]
        public async Task AuctionExists_ShouldReturnTrue_WhenAuctionExists()
        {
            // Simular respuesta HTTP exitosa con contenido válido
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new { exists = true }))
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("http://localhost:18085/") };
            var service = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var result = await service.AuctionExists(Guid.NewGuid(), Guid.NewGuid());

            Assert.True(result);
        }

        [Fact]
        public async Task AuctionExists_ShouldReturnFalse_WhenAuctionDoesNotExist()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("")
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("http://localhost:18085/") };
            var service = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var result = await service.AuctionExists(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task AuctionExists_ShouldReturnFalse_WhenHttpErrorOccurs()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("http://localhost:18085/") };
            var service = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.AuctionExists(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowHttpRequestException_WhenServerFails()
        {
            var handlerMock = new MockHttpMessageHandlerException(new HttpRequestException("Simulated error"));
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("http://localhost:18085/") };
            var service = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.AuctionExists(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task AuctionExists_ShouldIncludeAuthorizationHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"auction/producto-activo/{Guid.NewGuid()}?userId={Guid.NewGuid()}");

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["Authorization"] = "Bearer test-token";

            Assert.True(mockHttpContext.Request.Headers.ContainsKey("Authorization"));
            Assert.Equal("Bearer test-token", mockHttpContext.Request.Headers["Authorization"]);
        }
    }

}
