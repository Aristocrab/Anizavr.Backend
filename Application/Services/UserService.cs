using Application.Database;
using Application.Dtos;
using Application.Entities;
using Application.Exceptions;
using Application.Shared;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShikimoriSharp;
using ShikimoriSharp.Classes;
using Comment = Application.Entities.Comment;
using User = Application.Entities.User;

namespace Application.Services;

public class UserService
{
    private readonly UserDbContext _dbContext;
    private readonly ShikimoriClient _shikimoriClient;
    private readonly IValidator<RegisterDto> _registerDtoValidator;
    private readonly IValidator<LoginDto> _loginDtoValidator;

    public UserService(UserDbContext dbContext, ShikimoriClient shikimoriClient,
        IValidator<RegisterDto> registerDtoValidator, IValidator<LoginDto> loginDtoValidator)
    {
        _dbContext = dbContext;
        _shikimoriClient = shikimoriClient;
        _registerDtoValidator = registerDtoValidator;
        _loginDtoValidator = loginDtoValidator;
    }
    
    #region Authentication
    
    public async Task<Guid> Register(RegisterDto registerDto)
    {
        await _registerDtoValidator.ValidateAndThrowAsync(registerDto);
        
        if (_dbContext.Users.Any(x => x.Username == registerDto.Username))
        {
            throw new UserAlreadyExistsException(nameof(registerDto.Username), registerDto.Username);
        }
        if (_dbContext.Users.Any(x => x.Email == registerDto.Email))
        {
            throw new UserAlreadyExistsException(nameof(registerDto.Email), registerDto.Email);
        }

        var avatarUrl =
            $"https://api.dicebear.com/6.x/thumbs/svg?seed={registerDto.Username}&shapeColor=43aa52&backgroundColor=daf9d9";
        
        // if (registerDto.Avatar is not null)
        // {
        //     avatarUrl = await SaveAvatar(registerDto.Avatar, registerDto.Email);
        // }

        var (hash, salt) = Hashing.HashPassword(registerDto.Password);
        
        var user = new User
        {
            Email = registerDto.Email,
            Username = registerDto.Username,
            PasswordHash = hash,
            Salt = salt,
            CurrentlyWatchingAnime = new List<UserWatchingAnime>(),
            WatchedAnime = new List<UserWatchedAnime>(),
            Wishlist = new List<WishlistAnime>(),
            AvatarUrl = avatarUrl
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user.Id;
    }
    
    public async Task<Guid> Login(LoginDto loginDto)
    {
        await _loginDtoValidator.ValidateAndThrowAsync(loginDto);
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == loginDto.Email);
        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(loginDto.Email), loginDto.Email);
        }
        
        if (user.PasswordHash != Hashing.HashPassword(loginDto.Password, user.Salt))
        {
            throw new WrongPasswordException(loginDto.Email);
        }

        return user.Id;
    }
    
    #endregion
    
    #region Avatar Management
    
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
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        var newAvatarUrl = await SaveAvatar(avatar, user.Username);
        user.AvatarUrl = newAvatarUrl;
        await _dbContext.SaveChangesAsync();

        return newAvatarUrl;
    }
    
    #endregion
    
    #region User Information
    
    public async Task<List<UserDto>> GetUsersLeaderbord()
    {
        var users = await _dbContext.Users
            .Include(x => x.WatchedAnime)
            .OrderByDescending(x => x.WatchedAnime.Select(y => y.CurrentEpisode))
            .ProjectToType<UserDto>()
            .ToListAsync();
        
        return users;
    }

    public async Task<UserDto> GetUser(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .Include(x => x.WatchedAnime)
            .Include(x => x.Wishlist)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        return user.Adapt<UserDto>();
    }
    
    public async Task<UserDto> GetUser(string username)
    {
        var user = await _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .Include(x => x.WatchedAnime)
            .Include(x => x.Wishlist)
            .FirstOrDefaultAsync(x => x.Username == username);
        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(username), username);
        }

        return user.Adapt<UserDto>();
    }
    
    #endregion
    
    #region Anime Comments
    
    public async Task<List<CommentDto>> GetAnimeComments(long animeId)
    {
        var comments = await _dbContext.Comments
            .Where(x => x.AnimeId == animeId)
            .ProjectToType<CommentDto>()
            .ToListAsync();

        return comments;
    }
    
    public async Task AddCommentToAnime(Guid userId, long animeId, string text)
    {
        var user = _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        var comment = new Comment
        {
            AnimeId = animeId,
            Author = user,
            Text = text,
            Created = DateTime.Now
        };

        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
    }
    
    #endregion

    #region Watching list

    public async Task AddAnimeToWatchingList(Guid userId, long animeId, int currentEpisode, float secondsTotal)
    {
        var user = await GetUserById(userId);
        var anime = await GetAnimeById(animeId);

        var watchingAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);
        if (watchingAnime is not null)
        {
            watchingAnime.CurrentEpisode = currentEpisode;
            watchingAnime.SecondsTotal = secondsTotal;
        }
        else
        {
            var userAnime = CreateUserWatchingAnime(anime, currentEpisode, secondsTotal);
            user.CurrentlyWatchingAnime.Add(userAnime);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task FinishEpisode(Guid userId, long animeId, int episodeFinished)
    {
        var user = await GetUserById(userId);
        var userAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);

        if (userAnime is null)
        {
            throw new NotFoundException("Пользователь", nameof(animeId), animeId.ToString());
        }

        if (episodeFinished == userAnime.EpisodesTotal)
        {
            await RemoveAnimeFromWatchingList(userId, animeId);
            await AddAnimeToWatchedList(userId, animeId);
        }
        else
        {
            userAnime.CurrentEpisode = episodeFinished;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTimestamps(Guid userId, long animeId, int secondsWatched)
    {
        var user = await GetUserById(userId);
        var userAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);

        if (userAnime is null)
        {
            throw new NotFoundException("Пользователь", nameof(animeId), animeId.ToString());
        }

        userAnime.SecondsWatched = secondsWatched;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAnimeFromWatchingList(Guid userId, long animeId)
    {
        var user = await GetUserById(userId);
        var watchingAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);

        if (watchingAnime is null)
        {
            throw new NotFoundException("Аниме", nameof(animeId), animeId.ToString());
        }

        user.CurrentlyWatchingAnime.Remove(watchingAnime);
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Watched list

    public async Task AddAnimeToWatchedList(Guid userId, long animeId, int? userScore = null, int? currentEpisode = null)
    {
        var user = await GetUserById(userId);
        var anime = await GetAnimeById(animeId);

        if (user.WatchedAnime.All(x => x.AnimeId != animeId))
        {
            var userAnime = CreateUserWatchedAnime(anime, userScore, currentEpisode);
            user.WatchedAnime.Add(userAnime);
            await _dbContext.SaveChangesAsync();
        }
    }

    #endregion

    #region Wishlist

    public async Task AddAnimeToWishlist(Guid userId, long animeId)
    {
        var user = await GetUserById(userId);
        var anime = await GetAnimeById(animeId);

        if (user.Wishlist.All(x => x.AnimeId != animeId))
        {
            var userAnime = CreateWishlistAnime(anime);
            user.Wishlist.Add(userAnime);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAnimeFromWishlist(Guid userId, long animeId)
    {
        var user = await GetUserById(userId);
        var wishlistAnime = user.Wishlist.FirstOrDefault(x => x.AnimeId == animeId);

        if (wishlistAnime is null)
        {
            throw new NotFoundException("Аниме", nameof(animeId), animeId.ToString());
        }

        user.Wishlist.Remove(wishlistAnime);
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Helper Methods

    private async Task<User> GetUserById(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .Include(x => x.WatchedAnime)
            .Include(x => x.Wishlist)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        return user;
    }

    private async Task<AnimeID> GetAnimeById(long animeId)
    {
        try
        {
            return await _shikimoriClient.Animes.GetAnime(animeId);
        }
        catch
        {
            throw new NotFoundException("Аниме", nameof(animeId), animeId.ToString());
        }
    }

    private UserWatchingAnime CreateUserWatchingAnime(AnimeID anime, int currentEpisode, float secondsTotal)
    {
        return new UserWatchingAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            CurrentEpisode = currentEpisode,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            SecondsWatched = 0,
            SecondsTotal = secondsTotal
        };
    }

    private UserWatchedAnime CreateUserWatchedAnime(AnimeID anime, int? userScore, int? currentEpisode)
    {
        return new UserWatchedAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            CurrentEpisode = currentEpisode ?? (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            UserScore = userScore
        };
    }

    private WishlistAnime CreateWishlistAnime(AnimeID anime)
    {
        return new WishlistAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score
        };
    }

    #endregion

}