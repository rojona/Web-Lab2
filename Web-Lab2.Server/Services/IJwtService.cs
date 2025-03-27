using Web_Lab2.Models;

namespace Web_Lab2.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}