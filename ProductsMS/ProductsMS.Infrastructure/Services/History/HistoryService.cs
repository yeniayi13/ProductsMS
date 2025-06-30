using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ProductsMs.Infrastructure;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Core.Service.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Infrastructure.Services.History
{
    public class HistoryService : IHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;

        public HistoryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor,
            IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            //* Configuracion del HttpClient
            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()
                ?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18084/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task AddActivityHistoryAsync(GetHistoryDto history)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history), "La actividad no puede ser nula.");
            }

            var content = new StringContent(
                JsonSerializer.Serialize(history),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"user/activity-history/UserId/{history.UserId}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error al registrar la actividad: {response.StatusCode}");
            }

            Console.WriteLine($"Actividad registrada con éxito: {history.Id}");
        }
    }
}
