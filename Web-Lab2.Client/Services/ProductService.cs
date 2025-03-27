using System.Net.Http.Json;
using Web_Lab2.Client.Models;

namespace Web_Lab2.Client.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ProductService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/api/Products");
            var content = await response.Content.ReadAsStringAsync();
        
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Product>>() ?? new List<Product>();
            }
            else
            {
                return new List<Product>();
            }
        }
        catch (Exception ex)
        {
            return new List<Product>();
        }
    }

    public async Task<Product> GetProductAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Product>($"{ApiConfig.BaseUrl}/api/Products/{id}");
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>($"{ApiConfig.BaseUrl}/api/Products/search?name={searchTerm}") ?? new List<Product>();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        await SetAuthHeaderAsync();
        
        var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/Products", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task UpdateProductAsync(int id, Product product)
    {
        await SetAuthHeaderAsync();
        
        var response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/api/Products/{id}", product);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProductAsync(int id)
    {
        await SetAuthHeaderAsync();
        
        var response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/api/Products/{id}");
        response.EnsureSuccessStatusCode();
    }
}