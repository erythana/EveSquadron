using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EVEye.DataAccess.Base
{
    public abstract class RestDataAccessBase<T> 
    {
        private readonly HttpClient _httpClient;

        protected RestDataAccessBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<List<T>?> GetAllAsync(string endpointUrl)
        {
            var response = await _httpClient.GetAsync(endpointUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<T>>(content);

            return items;
        }

        protected async Task<T?> GetByIdAsync(string endpointUrl, int id)
        {
            var response = await _httpClient.GetAsync($"{endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<T>(content);

            return item;
        }

        protected async Task<T?> CreateAsync(string endpointUrl, T item)
        {
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
            var response = await _httpClient.DeleteAsync($"{endpointUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}