using Microsoft.AspNetCore.Mvc;
using Web_Lab2.DTOs;
using Web_Lab2.Services;

namespace Web_Lab2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserService userService, ILogger<UserService> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO model)
    {
        try
        {
            await userService.RegisterAsync(model);
            return Ok(new { message = "Registration successful" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO model)
    {
        try
        {
            logger.LogInformation($"Login request received for email {model.Email}");

            var (user, token) = await userService.LoginAsync(model);

            var userDto = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.Roles
            };

            logger.LogInformation($"Login successful for email {user.Email}");

            return Ok(new AuthResponseDTO
            {
                Token = token,
                User = userDto
            });
        }
        catch (ApplicationException ex)
        {
            logger.LogError($"Login failed for email {model.Email}", ex);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError($"Unexpected error during login");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}