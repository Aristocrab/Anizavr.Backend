using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Application.Services;
using Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Controllers.Shared;

namespace WebApi.Controllers;

[Authorize]
[Route("/api/users")]
public class UserController : BaseController
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    private static string GenerateJwtToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("UserId", userId.ToString())
        };

        var secretBytes = Encoding.UTF8.GetBytes(Constants.JwtSecretKey);
        var key = new SymmetricSecurityKey(secretBytes);
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: Constants.Audience,
            issuer: Constants.Issuer,
            claims: claims,
            notBefore: DateTime.Now,
            signingCredentials: signingCredentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<string> Register(RegisterDto registerDto)
    {
        var userId = await _userService.Register(registerDto);
        return GenerateJwtToken(userId);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<string> Login(LoginDto loginDto)
    {
        var userId = await _userService.Login(loginDto);
        return GenerateJwtToken(userId);
    }
    
    [HttpGet("getUser")]
    public async Task<UserDto> GetUser()
    {
        return await _userService.GetUser(UserId);
    }

    [HttpPost("addToWatching")]
    public async Task AddAnimeToWatchingList(long animeId)
    {
        await _userService.AddAnimeToWatchingList(UserId, animeId);
    }

    [HttpPut("finishEpisode")]
    public async Task FinishEpisode(long animeId, int episodeFinished)
    {
        await _userService.FinishEpisode(UserId, animeId, episodeFinished);
    }

    [HttpPut("updateTimestamps")]
    public async Task UpdateTimestamps(long animeId, int secondsWatched)
    {
        await _userService.UpdateTimestamps(UserId, animeId, secondsWatched);
    }

    [HttpDelete("removeFromWatching")]
    public async Task RemoveAnimeFromWatchingList(long animeId)
    {
        await _userService.RemoveAnimeFromWatchingList(UserId, animeId);
    }
}