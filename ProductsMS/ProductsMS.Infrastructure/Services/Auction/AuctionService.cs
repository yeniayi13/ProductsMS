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
    public class AuctionService: IAuctionService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;
        public AuctionService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            //* Configuracion del HttpClient
            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18084/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<bool> AuctionExists(Guid productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auction/product/{productId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al verificar si el producto está en una subasta: {response.StatusCode}");
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();

                if (responseStream == null)
                {
                    throw new InvalidOperationException("El contenido de la respuesta es nulo.");
                }

                string responseContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    throw new InvalidOperationException("El contenido de la respuesta es vacío o inválido.");
                }

                bool exists;
                try
                {
                    exists = JsonSerializer.Deserialize<bool>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException ex)
                {
                    throw new JsonException("Error al deserializar la respuesta JSON.", ex);
                }

                return exists;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine($"Operación inválida: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Error de deserialización JSON: {ex.Message}");
                throw;
            }
            catch (TaskCanceledException ex)
            {
                Console.Error.WriteLine($"La solicitud fue cancelada por tiempo de espera: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error inesperado: {ex.Message}");
                throw;
            }
        }
    }
}
