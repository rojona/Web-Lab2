using Microsoft.EntityFrameworkCore;
using Web_Lab2.Data;
using Web_Lab2.DTOs;
using Web_Lab2.Models;

namespace Web_Lab2.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, IJwtService jwtService, ILogger<UserService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }
    
    public async Task<User> RegisterAsync(RegisterUserDTO model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            throw new ApplicationException("Email already exists");
        }

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Roles = new List<string> { "User" }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<(User user, string token)> LoginAsync(LoginUserDTO model)
    {
        _logger.LogInformation($"Attempting to login: {model.Email}");
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            _logger.LogInformation($"User {model.Email} not found");
            throw new ApplicationException("Invalid email or password");
        }
        
        _logger.LogInformation($"User found: {user.Username}, Checking password...");
        
        bool passwordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

        if (!passwordValid)
        {
            _logger.LogInformation($"Login attempt failed. Invalid password for user {model.Email}");
            throw new ApplicationException("Invalid password");
        }
        
        _logger.LogInformation($"User {user.Email} logged in");

        var token = _jwtService.GenerateToken(user);

        return (user, token);
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
}