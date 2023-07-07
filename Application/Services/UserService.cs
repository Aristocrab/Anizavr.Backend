using Application.Database;
using Application.Dtos;
using Application.Entities;
using Application.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Http;
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
        if (_dbContext.Users.Any(x => x.Email == Email.From(registerDto.Email)))
        {
            throw new UserAlreadyExistsException(nameof(registerDto.Email), registerDto.Email);
        }

        var avatarUrl =
            $"https://api.dicebear.com/6.x/thumbs/svg?seed={registerDto.Username}&shapeColor=43aa52&backgroundColor=daf9d9";
        
        if (registerDto.Avatar is not null)
        {
            avatarUrl = await SaveAvatar(registerDto.Avatar, registerDto.Username);
        }
        
        var user = new User
        {
            Email = Email.From(registerDto.Email),
            Username = registerDto.Username,
            Password = registerDto.Password,
            CurrentlyWatchingAnime = new List<UserWatchingAnime>(),
            WatchedAnime = new List<UserWatchedAnime>(),
            AvatarUrl = avatarUrl
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
    
    private static async Task<string> SaveAvatar(IFormFile file, string username)
    {
        var avatarDirectory = $"{Directory.GetCurrentDirectory()}/Images/Avatars"; 
        Directory.CreateDirectory(avatarDirectory);

        var newFileName = $"{username}_{file.FileName}";
        var fullPath = $"{avatarDirectory}/{newFileName}";

        Console.WriteLine(fullPath);

        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        
        var avatarUrl = $"/avatars/{newFileName}";
        return avatarUrl;
    }
    
    public async Task<string> ChangeUserAvatar(Guid userId, IFormFile avatar)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }

        var newAvatarUrl = await SaveAvatar(avatar, user.Username);
        user.AvatarUrl = newAvatarUrl;
        await _dbContext.SaveChangesAsync();

        return newAvatarUrl;
    }
    
    public async Task<List<UserDto>> GetUsersLeaderbord()
    {
        var users = await _dbContext.Users
            .Include(x => x.WatchedAnime)
            .OrderByDescending(x => x.WatchedAnime.Select(y => y.EpisodesWatched))
            .ProjectToType<UserDto>()
            .ToListAsync();
        
        return users;
    }

    public async Task<UserDto> GetUser(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
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
            .Include(x => x.CurrentlyWatchingAnime)
            .FirstOrDefaultAsync(x => x.Username == username);
        if (user is null)
        {
            throw new NotFoundException(nameof(username), username);
        }

        return user.Adapt<UserDto>();
    }

    public async Task AddAnimeToWatchedList(Guid userId, long animeId, int? userScore = null, int? episodesWatched = null)
    {
        var user = _dbContext.Users
            .Include(x => x.WatchedAnime)
            .FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }

        var anime = await _shikimoriClient.Animes.GetAnime(animeId);

        var userAnime = new UserWatchedAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            EpisodesWatched = episodesWatched ?? (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            UserScore = userScore
        };
        
        user.WatchedAnime.Add(userAnime);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task AddAnimeToWatchingList(Guid userId, long animeId)
    {
        var user = _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }

        var anime = await _shikimoriClient.Animes.GetAnime(animeId);

        var userAnime = new UserWatchingAnime
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
        
        user.CurrentlyWatchingAnime.Add(userAnime);
        await _dbContext.SaveChangesAsync();
    }

    public async Task FinishEpisode(Guid userId, long animeId, int episodeFinished)
    {
        var user = _dbContext.Users.Include(x => x.CurrentlyWatchingAnime).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }
        
        var userAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);
        if (userAnime is null)
        {
            throw new NotFoundException(nameof(animeId), animeId.ToString());
        }

        if (userAnime.EpisodesTotal == episodeFinished)
        {
            await RemoveAnimeFromWatchingList(userId, animeId);
            await AddAnimeToWatchedList(userId, animeId); // todo ???
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
        var user = _dbContext.Users.Include(x => x.CurrentlyWatchingAnime).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }
        
        var userAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);
        if (userAnime is null)
        {
            throw new NotFoundException(nameof(animeId), animeId.ToString());
        }

        userAnime.SecondsWatched = secondsWatched;
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAnimeFromWatchingList(Guid userId, long animeId)
    {
        var user = _dbContext.Users.Include(x => x.CurrentlyWatchingAnime).FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException(nameof(userId), userId.ToString());
        }
        
        var userAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);
        if (userAnime is null)
        {
            throw new NotFoundException(nameof(animeId), animeId.ToString());
        }

        user.CurrentlyWatchingAnime.Remove(userAnime);
        await _dbContext.SaveChangesAsync();
    }
}