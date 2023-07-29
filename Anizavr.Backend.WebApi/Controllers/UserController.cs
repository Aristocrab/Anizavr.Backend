using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.Shared;
using Anizavr.Backend.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Anizavr.Backend.WebApi.Controllers;

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
    [HttpGet("getUser/{username}")]
    public async Task<UserDto> GetUser(string username)
    {
        return await _userService.GetUser(username);
    }
    
    [HttpGet("getCurrentUser")]
    public async Task<UserDto?> GetCurrentUser()
    {
        return await _userService.GetUser(UserId);
    }
    
    [AllowAnonymous]
    [HttpGet("getComments/{animeId}")]
    public async Task<List<CommentDto>> GetComments(long animeId)
    {
        return await _userService.GetAnimeComments(animeId);
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
    
    [AllowAnonymous]
    [HttpGet("getUsersLeaderbord")]
    public async Task<List<UserDto>> GetUsersLeaderbord()
    {
        return await _userService.GetUsersLeaderbord();
    }
    
    [HttpPost("changeAvatar")]
    public async Task<string> ChangeAvatar(IFormFile avatar)
    {
        return await _userService.ChangeUserAvatar(UserId, avatar);
    }
    
    [HttpPost("addComment")]
    public async Task AddComment([FromBody] AddCommentDto addCommentDto)
    {
        await _userService.AddCommentToAnime(UserId, addCommentDto.AnimeId, addCommentDto.Text);
    }
    
    [HttpDelete("deleteComment/{commentId}")]
    public async Task DeleteComment(Guid commentId)
    {
        await _userService.DeleteComment(UserId, commentId);
    }

    [HttpPost("addToWatched/{animeId}")]
    public async Task AddAnimeToWatchedList(long animeId, int? userScore = null)
    {
        await _userService.AddAnimeToWatchedList(UserId, animeId, userScore);
    }
    
    [HttpPost("addToWatching/{animeId}")]
    public async Task AddAnimeToWatchingList(long animeId, int currentEpisode, float secondsTotal)
    {
        await _userService.AddAnimeToWatchingList(UserId, animeId, currentEpisode, secondsTotal);
    }
    
    [HttpPost("addToWishlist/{animeId}")]
    public async Task AddAnimeToWishlist(long animeId)
    {
        await _userService.AddAnimeToWishlist(UserId, animeId);
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
    
    [HttpDelete("removeFromWishlist")]
    public async Task RemoveAnimeFromWishlist(long animeId)
    {
        await _userService.RemoveAnimeFromWishlist(UserId, animeId);
    }
    
    [HttpPost("addToTierlist/{animeId}")]
    public async Task AddAnimeToTierlist(long animeId)
    {
        await _userService.AddAnimeToTierlist(UserId, animeId);
    }
    
    [HttpPut("changeTierlistOrder/{animeId}")]
    public async Task ChangeTierlistOrder(long animeId, int newPosition)
    {
        await _userService.ChangeTierlistOrder(UserId, animeId, newPosition);
    }
    
    [HttpDelete("removeFromTierlist/{animeId}")]
    public async Task RemoveAnimeFromTierlist(long animeId)
    {
        await _userService.RemoveAnimeFromTierlist(UserId, animeId);
    }
}