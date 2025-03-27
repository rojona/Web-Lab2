using System.Net.Http.Json;
using Web_Lab2.Client.Models;

namespace Web_Lab2.Client.Services;

public class OrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Order>>($"{ApiConfig.BaseUrl}/api/Orders") ?? new List<Order>();
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Order>($"{ApiConfig.BaseUrl}/api/Orders/{id}");
    }

    public async Task<List<Order>> GetOrdersByCustomerAsync(int customerId)
    {
        return await _httpClient.GetFromJsonAsync<List<Order>>($"{ApiConfig.BaseUrl}/api/Orders/customer/{customerId}") ??
               new List<Order>();
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest orderRequest)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/Orders", orderRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Order>();
    }
}

    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public List<CreateOrderItemRequest> OrderItems { get; set; } = new List<CreateOrderItemRequest>();
    }

    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }