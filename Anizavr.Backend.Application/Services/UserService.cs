using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Entities;
using Anizavr.Backend.Application.Exceptions;
using Anizavr.Backend.Application.Shared;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShikimoriSharp.Classes;
using Comment = Anizavr.Backend.Application.Entities.Comment;
using User = Anizavr.Backend.Application.Entities.User;

namespace Anizavr.Backend.Application.Services;

public class UserService : IUserService
{
    private readonly IAnizavrDbContext _dbContext;
    private readonly IAnimeService _animeService;
    private readonly IValidator<RegisterDto> _registerDtoValidator;
    private readonly IValidator<LoginDto> _loginDtoValidator;
    
    private static readonly object Lock = new();

    public UserService(IAnizavrDbContext dbContext, IAnimeService animeService,
        IValidator<RegisterDto> registerDtoValidator, IValidator<LoginDto> loginDtoValidator)
    {
        _dbContext = dbContext;
        _animeService = animeService;
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
            Tierlist = new List<TierlistAnime>(),
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
        var user = await GetUserById(userId);
        
        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        return user.Adapt<UserDto>();
    }
    
    public async Task<UserDto> GetUser(string username)
    {
        var user = await GetUserByUsername(username);
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
    
    public async Task<Guid> AddCommentToAnime(Guid userId, long animeId, string text)
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
        
        return comment.Id; 
    }

    public async Task DeleteComment(Guid userId, Guid commentId)
    {
        var user = _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment is null)
        {
            throw new NotFoundException("Комментарий", nameof(commentId), commentId.ToString());
        }

        if (comment.Author.Id != userId)
        {
            throw new UnauthorizedException(userId, nameof(commentId), commentId.ToString());
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
    
    #endregion

    #region Watching list

    public async Task AddAnimeToWatchingList(Guid userId, long animeId, int currentEpisode, float secondsTotal)
    {
        var user = await GetUserById(userId);

        var watchingAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);
        if (watchingAnime is not null)
        {
            watchingAnime.CurrentEpisode = currentEpisode;
            watchingAnime.SecondsTotal = secondsTotal;
        }
        else
        {
            lock (Lock)
            {
                if(user.CurrentlyWatchingAnime.Any(x => x.AnimeId == animeId)) return;
                var anime = _animeService.GetShikimoriAnimeById(animeId).Result;
                var userAnime = CreateUserWatchingAnime(anime, currentEpisode, secondsTotal);
                user.CurrentlyWatchingAnime.Add(userAnime);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task FinishEpisode(Guid userId, long animeId, int episodeFinished)
    {
        var user = await GetUserById(userId);
        var userAnime = user.CurrentlyWatchingAnime.FirstOrDefault(x => x.AnimeId == animeId);

        if (userAnime is null)
        {
            throw new NotFoundException("Аниме", nameof(animeId), animeId.ToString());
        }

        if (episodeFinished == userAnime.EpisodesTotal)
        {
            await RemoveAnimeFromWatchingList(userId, animeId);
            if (user.WatchedAnime.All(x => x.AnimeId != animeId))
            {
                await AddAnimeToWatchedList(userId, animeId);
            }
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

        _dbContext.UserWatchingAnimeList.Remove(watchingAnime);
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Watched list

    public async Task AddAnimeToWatchedList(Guid userId, long animeId, int? userScore = null, int? currentEpisode = null)
    {
        var user = await GetUserById(userId);
        var anime = await _animeService.GetShikimoriAnimeById(animeId);
        
        lock (Lock)
        {
            if (user.WatchedAnime.All(x => x.AnimeId != animeId))
            {
                    var userAnime = CreateUserWatchedAnime(anime, userScore, currentEpisode);
                    user.WatchedAnime.Add(userAnime);
                    _dbContext.SaveChanges();
            }
        }
    }

    #endregion

    #region Wishlist

    public async Task AddAnimeToWishlist(Guid userId, long animeId)
    {
        var user = await GetUserById(userId);
        var anime = await _animeService.GetShikimoriAnimeById(animeId);

        lock (Lock)
        {
            if (user.Wishlist.All(x => x.AnimeId != animeId))
            {
                var userAnime = CreateWishlistAnime(anime);
                user.Wishlist.Add(userAnime);
                _dbContext.SaveChanges();
            }
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

        _dbContext.Wishlist.Remove(wishlistAnime);
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Tierlist
    
    public async Task AddAnimeToTierlist(Guid userId, long animeId)
    {
        var user = await GetUserById(userId);
        var anime = await _animeService.GetShikimoriAnimeById(animeId);
        var tierlist = user.Tierlist.OrderBy(x => x.Position).Select(x => x.Position).LastOrDefault();

        lock (Lock)
        {
            if (user.Tierlist.All(x => x.AnimeId != animeId))
            {
                var userAnime = CreateTierlistAnime(anime, tierlist+1);
                user.Tierlist.Add(userAnime);
                _dbContext.SaveChanges();
            }
        }
    }

    public async Task ChangeTierlistOrder(Guid userId, long animeId, int newPosition)
    {
        var user = await GetUserById(userId);
        
        if(user.Tierlist.All(x => x.AnimeId != animeId)) return;

        var anime = user.Tierlist.First(x => x.AnimeId == animeId);
        var oldPosition = anime.Position;
        
        var animeToSwap = user.Tierlist.FirstOrDefault(x => x.Position == newPosition);
        if (animeToSwap is not null)
        {
            animeToSwap.Position = oldPosition;
        }

        anime.Position = newPosition;

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAnimeFromTierlist(Guid userId, long animeId)
    {
        var user = await GetUserById(userId);
        var tierlistAnime = user.Tierlist.FirstOrDefault(x => x.AnimeId == animeId);

        if (tierlistAnime is null)
        {
            throw new NotFoundException("Аниме", nameof(animeId), animeId.ToString());
        }

        _dbContext.Tierlist.Remove(tierlistAnime);
        
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
            .Include(x => x.Tierlist)
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(userId), userId.ToString());
        }

        user.Tierlist = user.Tierlist.OrderBy(x => x.Position).ToList();

        return user;
    }
    
    private async Task<User> GetUserByUsername(string username)
    {
        var user = await _dbContext.Users
            .Include(x => x.CurrentlyWatchingAnime)
            .Include(x => x.WatchedAnime)
            .Include(x => x.Wishlist)
            .Include(x => x.Tierlist)
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user is null)
        {
            throw new NotFoundException("Пользователь", nameof(username), username);
        }

        user.Tierlist = user.Tierlist.OrderBy(x => x.Position).ToList();

        return user;
    }

    private static UserWatchingAnime CreateUserWatchingAnime(AnimeID anime, int currentEpisode, float secondsTotal)
    {
        if (anime.Id == AnimeHelper.DeathNoteId)
        {
            AnimeHelper.FixDeathNotePoster(anime);
        }

        return new UserWatchingAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            CurrentEpisode = currentEpisode,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            SecondsWatched = 0,
            SecondsTotal = secondsTotal,
            Kind = anime.Kind
        };
    }

    private static UserWatchedAnime CreateUserWatchedAnime(AnimeID anime, int? userScore, int? currentEpisode)
    {
        if (anime.Id == AnimeHelper.DeathNoteId)
        {
            AnimeHelper.FixDeathNotePoster(anime);
        }
        
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

    private static WishlistAnime CreateWishlistAnime(AnimeID anime)
    {
        if (anime.Id == AnimeHelper.DeathNoteId)
        {
            AnimeHelper.FixDeathNotePoster(anime);
        }
        
        return new WishlistAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            Kind = anime.Kind
        };
    }
    
    private static TierlistAnime CreateTierlistAnime(AnimeID anime, int position)
    {
        if (anime.Id == AnimeHelper.DeathNoteId)
        {
            AnimeHelper.FixDeathNotePoster(anime);
        }
        
        return new TierlistAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            Position = position
        };
    }

    #endregion

}