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
                    throw new HttpRequestException($"Error al verificar si ese producto se encuentra en una subasta: {response.StatusCode}");
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                if (response.Content == null || string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()))
                {
                    throw new InvalidOperationException("El contenido de la respuesta es nulo o vacío.");
                }


                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
