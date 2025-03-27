using System.Net.Http.Json;
using Web_Lab2.Client.Models;

namespace Web_Lab2.Client.Services;

public class CustomerService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public CustomerService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            return await _httpClient.GetFromJsonAsync<List<Customer>>($"{ApiConfig.BaseUrl}/api/Customers") ?? new List<Customer>();
        }
        catch (Exception ex)
        {
            return new List<Customer>();
        }
    }

    public async Task<Customer> GetCustomerAsync(int id)
    {
        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        return await _httpClient.GetFromJsonAsync<Customer>($"{ApiConfig.BaseUrl}/api/Customers/{id}");
    }

    public async Task<Customer> GetCustomerByEmailAsync(string email)
    {
        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    
        try
        {
            var requestUrl = $"{ApiConfig.BaseUrl}/api/Customers/email/{Uri.EscapeDataString(email)}";
        
            var response = await _httpClient.GetAsync(requestUrl);
            var content = await response.Content.ReadAsStringAsync();
        
            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<Customer>();
                return customer;
            }
            else
            {
                throw new Exception($"Failed to get customer by email: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetCustomerByEmailAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/Customers", customer);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Customer>();
            }
            else
            {
                throw new Exception($"Failed to create customer: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in CreateCustomerAsync: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateCustomerAsync(int id, Customer customer)
    {
        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/api/Customers/{id}", customer);
        response.EnsureSuccessStatusCode();
    }
}