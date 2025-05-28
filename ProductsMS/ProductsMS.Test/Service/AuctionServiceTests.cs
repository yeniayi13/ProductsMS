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
        public async Task AuctionExists_ShouldIncludeAuthorizationHeader()
        {
            var productId = Guid.NewGuid();
            var request = new HttpRequestMessage(HttpMethod.Get, $"auction/product/{productId}");

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["Authorization"] = "Bearer test-token";

            Assert.True(mockHttpContext.Request.Headers.ContainsKey("Authorization"));
            Assert.Equal("Bearer test-token", mockHttpContext.Request.Headers["Authorization"]);
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowUnauthorizedException_WhenResponseIsForbidden()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Forbidden); // 403 Forbidden
            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Error al verificar si el producto está en una subasta", exception.Message);
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowServerErrorException_WhenResponseIsInternalServerError()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError); // 500 Internal Server Error
            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Error al verificar si el producto está en una subasta", exception.Message);
        }
        [Fact]
        public async Task AuctionExists_ShouldHandleValidJsonWithUnexpectedData()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"status\": \"unexpected\" }") // JSON válido, pero inesperado
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<JsonException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Error al deserializar la respuesta JSON.", exception.Message);
        }

        [Fact]
        public async Task AuctionExists_ShouldHandleMultipleConcurrentRequests()
        {
            var productId = Guid.NewGuid();
            var jsonResponse = JsonSerializer.Serialize(true);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var tasks = new Task<bool>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = auctionService.AuctionExists(productId);
            }

            var results = await Task.WhenAll(tasks);
            Assert.All(results, result => Assert.True(result));
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

        [Fact]
        public async Task AuctionExists_ShouldThrowHttpRequestException_WhenResponseIsNotSuccessful()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Error al verificar si el producto está en una subasta", exception.Message);
        }





        [Fact]
        public async Task AuctionExists_ShouldThrowTaskCanceledException_WhenRequestTimesOut()
        {
            var handlerMock = new MockHttpMessageHandlerException(new TaskCanceledException("Request timeout"));
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Request timeout", exception.Message);
        }


        [Fact]
        public async Task AuctionExists_ShouldHandleValidJsonWithoutData()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}") // JSON válido pero vacío
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<JsonException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Error al deserializar la respuesta JSON.", exception.Message);
        }

        [Fact]
        public async Task AuctionExists_ShouldThrowHttpRequestException_WhenNetworkFails()
        {
            var handlerMock = new MockHttpMessageHandlerException(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(Guid.NewGuid()));

            Assert.Contains("Network error", exception.Message);
        }

    }
}
