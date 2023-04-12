using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EVEye.Models;
using Microsoft.Extensions.Logging;

namespace EVEye.DataAccess.Base;

public abstract class RestDataAccessBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RestDataAccessBase> _logger;

    protected RestDataAccessBase(HttpClient httpClient, ILogger<RestDataAccessBase> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    protected async Task<IEnumerable<T>> GetAllAsync<T>(string endpointUrl)
    {
        _logger.LogDebug($"Executing GetAllAsync on endpoint {endpointUrl}");
        var content = await _httpClient.GetStringAsync(endpointUrl);
        return JsonSerializer.Deserialize<List<T>>(content, ApplicationConstants.AppDefaultSerializerOptions)!;
    }

    protected async Task<IEnumerable<T>> GetAllPOSTAsync<T>(string endpointUrl, HttpContent payload)
    {
        _logger.LogDebug($"Executing GetAllPOSTAsync (string jsonPayload) on endpoint {endpointUrl}");

        var response = await _httpClient.PostAsync(endpointUrl, payload);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<T>>(content, ApplicationConstants.AppDefaultSerializerOptions)!;
    }

    protected async Task<T> GetByIdAsync<T>(string endpointUrl, int? id)
    {
        _logger.LogDebug($"Executing GetByIdAsync on endpoint {endpointUrl}{id}/");
        var content = await _httpClient.GetStringAsync($"{endpointUrl}{id}/");
        
        return JsonSerializer.Deserialize<T>(content, ApplicationConstants.AppDefaultSerializerOptions)!;
    }

    protected async Task<T> CreateAsync<T>(string endpointUrl, T item)
    {
        _logger.LogDebug($"Executing CreateAsync (T item) on endpoint {endpointUrl}");

        var json = JsonSerializer.Serialize(item);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpointUrl, content);
        response.EnsureSuccessStatusCode();

        var createdContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(createdContent, ApplicationConstants.AppDefaultSerializerOptions)!;
    }

    protected async Task<T> CreateAsync<T>(string endpointUrl, HttpContent jsonPayload)
    {
        _logger.LogDebug($"Executing CreateAsync (string jsonPayload) on endpoint {endpointUrl}");
        
        var response = await _httpClient.PostAsync(endpointUrl, jsonPayload);
        response.EnsureSuccessStatusCode();

        var createdContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(createdContent, ApplicationConstants.AppDefaultSerializerOptions)!;
    }

    protected async Task<T> UpdateAsync<T>(string endpointUrl, int id, T item)
    {
        _logger.LogDebug($"Executing UpdateAsync on endpoint {endpointUrl}");

        var json = JsonSerializer.Serialize(item);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{endpointUrl}/{id}", content);
        response.EnsureSuccessStatusCode();

        var updatedContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(updatedContent, ApplicationConstants.AppDefaultSerializerOptions)!;
    }

    protected async Task DeleteAsync(string endpointUrl, int id)
    {
        _logger.LogDebug($"Executing DeleteAsync on endpoint {endpointUrl}");

        var response = await _httpClient.DeleteAsync($"{endpointUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }
}