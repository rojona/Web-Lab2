namespace Web_Lab2.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
}

public class RegisterUserDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginUserDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class AuthResponseDTO
{
    public string? Token { get; set; }
    public UserDTO? User { get; set; }
}