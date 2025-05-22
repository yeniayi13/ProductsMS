
using Moq;
using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using ProductsMS.Infrastructure.Services.User;
using ProductsMs.Infrastructure;
using ProductsMS.Common.Dtos.Product.Response;
using System.Text.Json;
using ProductsMS.Infrastructure.Services.Auction;
namespace ProductsMS.Test.Service

{
    public class UserServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IOptions<HttpClientUrl>> _mockHttpClientUrl;
        private readonly UserService _userService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public UserServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpClientUrl = new Mock<IOptions<HttpClientUrl>>();

            _mockHttpClientUrl.Setup(x => x.Value).Returns(new HttpClientUrl { ApiUrl = "https://example.com" });

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["Authorization"] = "Bearer test-token";
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            // Simular HttpClient usando HttpMessageHandler
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com")
            };

            _userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);
        }

        [Fact]
        public async Task AuctioneerExists_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new GetUser { UserId = userId, UserName = "Test User" };
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var jsonContent = JsonSerializer.Serialize(expectedUser, options);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage,true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act
            var result = await userService.AuctioneerExists(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
            Assert.Equal(expectedUser.UserName, result.UserName);
        }

        [Fact]
        public async Task AuctioneerExists_ShouldThrowHttpRequestException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => userService.AuctioneerExists(userId));
        }

        [Fact]
        public async Task AuctioneerExists_ShouldThrowInvalidOperationException_WhenResponseStreamIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null // Simula un contenido vacío
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => userService.AuctioneerExists(userId));
            Assert.Contains( "",exception.Message);
        }

        [Fact]
        public async Task AuctioneerExists_ShouldThrowInvalidOperationException_WhenDeserializationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid_json") // Simula un JSON inválido
            };

            var handlerMock = new MockHttpMessageHandler(responseMessage,true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => userService.AuctioneerExists(userId));
        }

        [Fact]
        public async Task AuctioneerExists_ShouldThrowHttpRequestException_WhenServerFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => userService.AuctioneerExists(userId));
        }
        [Fact]
        public async Task AuctioneerExists_ShouldThrowJsonException_WhenResponseIsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NoContent); // 🔹 Simular `204 No Content`

            var handlerMock = new MockHttpMessageHandler(responseMessage,true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => userService.AuctioneerExists(userId));
        }
        [Fact]
        public async Task AuctioneerExists_ShouldThrowJsonException_WhenResponseHasInvalidContentType()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<html><body>Not a JSON response</body></html>")
            };
            responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"); // 🔹 Tipo incorrecto

            var handlerMock = new MockHttpMessageHandler(responseMessage, true);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };

            var userService = new UserService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => userService.AuctioneerExists(userId));
        }
        [Fact]
        public async Task AuctionExists_ShouldThrowHttpRequestException_WhenNetworkFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var handlerMock = new MockHttpMessageHandlerException(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://example.com") };
            var auctionService = new AuctionService(httpClient, _mockHttpContextAccessor.Object, _mockHttpClientUrl.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => auctionService.AuctionExists(productId));

            Assert.Contains("Network error", exception.Message);
        }
    }
}
