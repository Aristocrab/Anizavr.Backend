using Application.Database;
using Application.Dtos;
using Application.Entities;
using Application.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ShikimoriSharp;

namespace Application.Services;

public class UserService
{
    private readonly UserDbContext _dbContext;
    private readonly ShikimoriClient _shikimoriClient;

    public UserService(UserDbContext dbContext, ShikimoriClient shikimoriClient)
    {
        _dbContext = dbContext;
        _shikimoriClient = shikimoriClient;
    }

    public async Task<Guid> Register(RegisterDto registerDto)
    {
        if (_dbContext.Users.Any(x => x.Username == registerDto.Username))
        {
            throw new UserAlreadyExistsException(nameof(registerDto.Username), registerDto.Username);
        }
        if (_dbContext.Users.Any(x => x.Email == registerDto.Email))
        {
            throw new UserAlreadyExistsException(nameof(registerDto.Email), registerDto.Email);
        }
        
        var user = new User
        {
            Email = registerDto.Email,
            Username = registerDto.Username,
            Password = registerDto.Password,
            LastWatchedAnimes = new List<UserAnime>(),
            AvatarUrl = string.IsNullOrEmpty(registerDto.AvatarUrl) 
                ? "https://api.dicebear.com/6.x/thumbs/svg" +
                    $"?seed={registerDto.Username}&shapeColor=43aa52&backgroundColor=daf9d9" 
                : registerDto.AvatarUrl // todo
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<Guid> Login(LoginDto loginDto)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Username == loginDto.Username);
        if (user is null)
        {
            throw new NotFoundException(nameof(loginDto.Username), loginDto.Username);
        }
        if (user.Password != loginDto.Password)
        {
            throw new WrongPasswordException(loginDto.Username);
        }

        return user.Id;
    }

    public async Task<UserDto> GetUser(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(x => x.LastWatchedAnimes)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }

        return user.Adapt<UserDto>();
    }
    
    public async Task<UserDto> GetUser(string username)
    {
        var user = await _dbContext.Users
            .Include(x => x.LastWatchedAnimes)
            .FirstOrDefaultAsync(x => x.Username == username);
        if (user is null)
        {
            throw new NotFoundException(nameof(username), username);
        }

        return user.Adapt<UserDto>();
    }

    public async Task AddAnimeToWatchingList(Guid userId, long animeId)
    {
        var user = _dbContext.Users.Include(x => x.LastWatchedAnimes).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }

        var anime = await _shikimoriClient.Animes.GetAnime(animeId);

        var userAnime = new UserAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            EpisodesWatched = 0,
            NextEpisode = 1,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            SecondsWatched = 0
        };
        
        user.LastWatchedAnimes.Add(userAnime);
        await _dbContext.SaveChangesAsync();
    }

    public async Task FinishEpisode(Guid userId, long animeId, int episodeFinished)
    {
        var user = _dbContext.Users.Include(x => x.LastWatchedAnimes).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }
        
        var userAnime = user.LastWatchedAnimes.FirstOrDefault(x => x.AnimeId == animeId);
        if (userAnime is null)
        {
            throw new NotFoundException(nameof(animeId), animeId.ToString());
        }

        if (userAnime.EpisodesTotal == episodeFinished)
        {
            await RemoveAnimeFromWatchingList(userId, animeId);
        }
        else
        {
            userAnime.EpisodesWatched = episodeFinished;
            userAnime.NextEpisode = episodeFinished + 1;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateTimestamps(Guid userId, long animeId, int secondsWatched)
    {
        var user = _dbContext.Users.Include(x => x.LastWatchedAnimes).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }
        
        var userAnime = user.LastWatchedAnimes.FirstOrDefault(x => x.AnimeId == animeId);
        if (userAnime is null)
        {
            throw new NotFoundException(nameof(animeId), animeId.ToString());
        }

        userAnime.SecondsWatched = secondsWatched;
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAnimeFromWatchingList(Guid userId, long animeId)
    {
        var user = _dbContext.Users.Include(x => x.LastWatchedAnimes).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }
        
        var userAnime = user.LastWatchedAnimes.FirstOrDefault(x => x.AnimeId == animeId);
        if (userAnime is null)
        {
            throw new NotFoundException(nameof(animeId), animeId.ToString());
        }

        user.LastWatchedAnimes.Remove(userAnime);
        await _dbContext.SaveChangesAsync();
    }
}