using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Web_Lab2.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new { Email = email, Password = password };
            var requestUrl = $"{ApiConfig.BaseUrl}/api/Auth/login";

            var response = await _httpClient.PostAsJsonAsync(requestUrl, loginData);

            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", authResult.Token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", JsonSerializer.Serialize(authResult.User));

                await _jsRuntime.InvokeVoidAsync("notifyAuthStateChanged");

                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var registerData = new { Username = username, Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/Auth/register", registerData);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user");
        await _jsRuntime.InvokeVoidAsync("notifyAuthStateChanged");
    }

    public async Task<User> GetCurrentUserAsync()
    {
        try
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user");
            if (string.IsNullOrEmpty(userJson))
                return null!;

            return JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null!;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        return user?.Roles?.Contains(role) == true;
    }

    public async Task<string> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }
}

public class AuthResponse
{
    public string? Token { get; set; }
    public User? User { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
}