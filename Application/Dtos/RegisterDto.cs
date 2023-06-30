namespace Application.Dtos;

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string? AvatarUrl { get; set; }
}