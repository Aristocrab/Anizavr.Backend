using Anizavr.Backend.Application.Dtos;

namespace Anizavr.Backend.Application.Services;

public interface IUserService
{
    Task<Guid> Register(RegisterDto registerDto);
    Task<Guid> Login(LoginDto loginDto);
    
    Task<List<UserDto>> GetUsersLeaderbord();
    Task<UserDto> GetUser(Guid userId);
    Task<UserDto> GetUser(string username);
    
    Task<List<CommentDto>> GetAnimeComments(long animeId);
    Task<Guid> AddCommentToAnime(Guid userId, long animeId, string text);
    Task DeleteComment(Guid userId, Guid commentId);
    
    Task AddAnimeToWatchingList(Guid userId, long animeId, int currentEpisode, float secondsTotal);
    Task RemoveAnimeFromWatchingList(Guid userId, long animeId);
    
    Task FinishEpisode(Guid userId, long animeId, int episodeFinished);
    Task UpdateTimestamps(Guid userId, long animeId, int secondsWatched);
    
    Task AddAnimeToWatchedList(Guid userId, long animeId, int? userScore = null, int? currentEpisode = null);
    
    Task AddAnimeToWishlist(Guid userId, long animeId);
    Task RemoveAnimeFromWishlist(Guid userId, long animeId);
    
    Task AddAnimeToTierlist(Guid userId, long animeId);
    Task ChangeTierlistOrder(Guid userId, long animeId, int newPosition);
    Task RemoveAnimeFromTierlist(Guid userId, long animeId);
}