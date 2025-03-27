using Web_Lab2.DTOs;
using Web_Lab2.Models;

namespace Web_Lab2.Services;

public interface IUserService
{
    Task<User> RegisterAsync(RegisterUserDTO model);
    Task<(User user, string token)> LoginAsync(LoginUserDTO model);
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
}