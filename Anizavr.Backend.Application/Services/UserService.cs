﻿using Anizavr.Backend.Application.Common;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Interfaces;
using Anizavr.Backend.Domain.Entities;
using Anizavr.Backend.Domain.Exceptions;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Comment = Anizavr.Backend.Domain.Entities.Comment;
using User = Anizavr.Backend.Domain.Entities.User;

namespace Anizavr.Backend.Application.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _dbContext;
    private readonly IAnimeService _animeService;
    private readonly IValidator<RegisterDto> _registerDtoValidator;
    private readonly IValidator<LoginDto> _loginDtoValidator;
    
    private static readonly object Lock = new();

    public UserService(IAppDbContext dbContext, IAnimeService animeService,
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

        var (hash, salt) = HashingHelper.HashPassword(registerDto.Password);
        
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
        
        if (user.PasswordHash != HashingHelper.HashPassword(loginDto.Password, user.Salt))
        {
            throw new WrongPasswordException();
        }

        return user.Id;
    }
    
    #endregion
    
    #region User Information
    
    public async Task<List<UserDto>> GetUsersLeaderboard()
    {
        var users = await _dbContext.Users
            .Include(x => x.WatchedAnime)
            .OrderByDescending(x => x.WatchedAnime.Count)
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
    
    #region AnimeDto Comments
    
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
                var userAnime = UserAnimeFactory.CreateUserWatchingAnime(anime, currentEpisode, secondsTotal);
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
                    var userAnime = UserAnimeFactory.CreateUserWatchedAnime(anime, userScore, currentEpisode);
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
                var userAnime = UserAnimeFactory.CreateWishlistAnime(anime);
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
        var tierlist = user.Tierlist.OrderBy(x => x.Position).Select(x => x.Position)
            .LastOrDefault();

        lock (Lock)
        {
            if (user.Tierlist.All(x => x.AnimeId != animeId))
            {
                var userAnime = UserAnimeFactory.CreateTierlistAnime(anime, tierlist+1);
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

    #endregion
}