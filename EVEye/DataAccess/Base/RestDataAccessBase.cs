using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess.Base
{
    public abstract class RestDataAccessBase<T> 
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestDataAccessBase<T>> _logger;

        protected RestDataAccessBase(HttpClient httpClient, ILogger<RestDataAccessBase<T>> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected async Task<List<T>?> GetAllAsync(string endpointUrl)
        {
            _logger.LogDebug($"Executing GetAllAsync on endpoint {endpointUrl}");
            
            var response = await _httpClient.GetAsync(endpointUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<T>>(content);

            return items;
        }

        protected async Task<T?> GetByIdAsync(string endpointUrl, int id)
        {
            _logger.LogDebug($"Executing GetByIdAsync on endpoint {endpointUrl}");

            var response = await _httpClient.GetAsync($"{endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<T>(content);

            return item;
        }

        protected async Task<T?> CreateAsync(string endpointUrl, T item)
        {
            _logger.LogDebug($"Executing CreateAsync on endpoint {endpointUrl}");

            var json = JsonSerializer.Serialize(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpointUrl, content);
            response.EnsureSuccessStatusCode();

            var createdContent = await response.Content.ReadAsStringAsync();
            var createdItem = JsonSerializer.Deserialize<T>(createdContent);

            return createdItem;
        }

        protected async Task<T?> UpdateAsync(string endpointUrl, int id, T item)
        {
            _logger.LogDebug($"Executing UpdateAsync on endpoint {endpointUrl}");

            var json = JsonSerializer.Serialize(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{endpointUrl}/{id}", content);
            response.EnsureSuccessStatusCode();

            var updatedContent = await response.Content.ReadAsStringAsync();
            var updatedItem = JsonSerializer.Deserialize<T>(updatedContent);

            return updatedItem;
        }

        protected async Task DeleteAsync(string endpointUrl, int id)
        {
            _logger.LogDebug($"Executing DeleteAsync on endpoint {endpointUrl}");

            var response = await _httpClient.DeleteAsync($"{endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}