using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.WebApi.Configuration;
using Anizavr.Backend.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Anizavr.Backend.WebApi.Controllers;

[Authorize]
[Route("/api/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly IWebApiConfiguration _configuration;

    public UserController(IUserService userService, IWebApiConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }
    
    private string GenerateJwtToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("UserId", userId.ToString())
        };

        var secretBytes = Encoding.UTF8.GetBytes(_configuration.JwtSecretKey);
        var key = new SymmetricSecurityKey(secretBytes);
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: _configuration.JwtAudience,
            issuer: _configuration.JwtIssuer,
            claims: claims,
            notBefore: DateTime.Now,
            signingCredentials: signingCredentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }
    
    [AllowAnonymous]
    [HttpGet("getUser/{username}")]
    public Task<UserDto> GetUser(string username)
    {
        return _userService.GetUser(username);
    }
    
    [HttpGet("getCurrentUser")]
    public Task<UserDto> GetCurrentUser()
    {
        return _userService.GetUser(UserId);
    }
    
    [AllowAnonymous]
    [HttpGet("getComments/{animeId}")]
    public Task<List<CommentDto>> GetComments(long animeId)
    {
        return _userService.GetAnimeComments(animeId);
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
    public Task<List<UserDto>> GetUsersLeaderboard()
    {
        return _userService.GetUsersLeaderboard();
    }
    
    [HttpPost("addComment")]
    public Task AddComment([FromBody] AddCommentDto addCommentDto)
    {
        return _userService.AddCommentToAnime(UserId, addCommentDto.AnimeId, addCommentDto.Text);
    }
    
    [HttpDelete("deleteComment/{commentId:guid}")]
    public Task DeleteComment(Guid commentId)
    {
        return _userService.DeleteComment(UserId, commentId);
    }

    [HttpPost("addToWatched/{animeId:long}")]
    public Task AddAnimeToWatchedList(long animeId, int? userScore = null)
    {
        return _userService.AddAnimeToWatchedList(UserId, animeId, userScore);
    }
    
    [HttpPost("addToWatching/{animeId:long}")]
    public Task AddAnimeToWatchingList(long animeId, int currentEpisode, float secondsTotal)
    {
        return _userService.AddAnimeToWatchingList(UserId, animeId, currentEpisode, secondsTotal);
    }
    
    [HttpPost("addToWishlist/{animeId:long}")]
    public Task AddAnimeToWishlist(long animeId)
    {
        return _userService.AddAnimeToWishlist(UserId, animeId);
    }

    [HttpPut("finishEpisode")]
    public Task FinishEpisode(long animeId, int episodeFinished)
    {
        return _userService.FinishEpisode(UserId, animeId, episodeFinished);
    }

    [HttpPut("updateTimestamps")]
    public Task UpdateTimestamps(long animeId, int secondsWatched)
    {
        return _userService.UpdateTimestamps(UserId, animeId, secondsWatched);
    }

    [HttpDelete("removeFromWatching")]
    public Task RemoveAnimeFromWatchingList(long animeId)
    {
        return _userService.RemoveAnimeFromWatchingList(UserId, animeId);
    }
    
    [HttpDelete("removeFromWishlist")]
    public Task RemoveAnimeFromWishlist(long animeId)
    {
        return _userService.RemoveAnimeFromWishlist(UserId, animeId);
    }
    
    [HttpPost("addToTierlist/{animeId:long}")]
    public Task AddAnimeToTierlist(long animeId)
    {
        return _userService.AddAnimeToTierlist(UserId, animeId);
    }
    
    [HttpPut("changeTierlistOrder/{animeId:long}")]
    public Task ChangeTierlistOrder(long animeId, int newPosition)
    {
        return _userService.ChangeTierlistOrder(UserId, animeId, newPosition);
    }
    
    [HttpDelete("removeFromTierlist/{animeId:long}")]
    public Task RemoveAnimeFromTierlist(long animeId)
    {
        return _userService.RemoveAnimeFromTierlist(UserId, animeId);
    }
}