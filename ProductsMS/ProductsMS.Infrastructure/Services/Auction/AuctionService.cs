using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Infrastructure;
using ProductsMS.Core.Service.Auction;

namespace ProductsMS.Infrastructure.Services.Auction
{
    public class AuctionService : IAuctionService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;

        public AuctionService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor,
            IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            //* Configuracion del HttpClient
            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()
                ?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18085/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<bool> AuctionExists(Guid productId, Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auction/producto-activo/{productId}?userId={userId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Error obteniendo la información de la subasta: {response.StatusCode}");
                }

                // Leer el contenido y validar si es vacío o nulo
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    return false;
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                throw; // 🔹 Lanza la excepción en lugar de retornar `false`
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error inesperado: {ex.Message}");
                throw; // 🔹 Lanza la excepción en lugar de retornar `false`
            }
        }
    }
}
