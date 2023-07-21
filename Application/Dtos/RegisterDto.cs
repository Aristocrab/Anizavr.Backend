namespace Application.Dtos;

public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    // public IFormFile? Avatar { get; set; }
}