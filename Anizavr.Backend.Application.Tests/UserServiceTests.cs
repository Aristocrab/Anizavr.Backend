using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.Validators;
using AutoFixture;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ShikimoriSharp.Classes;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace Anizavr.Backend.Application.Tests;

public class UserServiceTests
{
    private readonly IAnizavrDbContext _dbContext;
    private readonly IUserService _userService;
    
    public UserServiceTests()
    {
        var contextOptions = new DbContextOptionsBuilder<AnizavrDbContext>()
            .UseInMemoryDatabase("AnizavrDb")
            .Options;

        _dbContext = new AnizavrDbContext(contextOptions);

        var animeService = Substitute.For<IAnimeService>();
        var fixture = new Fixture();
        var anime = fixture.Create<AnimeID>();
        anime.Id = 1;
        anime.Episodes = 26;
        animeService.GetShikimoriAnimeById(Arg.Any<long>()).Returns(Task.FromResult(anime));
        var registerDtoValidator = new RegisterDtoValidatior();
        var loginDtoValidator = new LoginDtoValidator();
        _userService = new UserService(_dbContext, animeService, registerDtoValidator, loginDtoValidator);
    }
    
    [Fact]
    public async Task Register_ShouldReturnUserId_WhenRegisterDtoIsValid()
    {
        // Arrange
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        
        // Act
        var result = await _userService.Register(registerDto);
        
        // Assert
        result.Should().NotBe(Guid.Empty);
        _dbContext.Users
            .Where(x => x.Id == result)
            .Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Register_ShouldThrowValidationException_WhenRegisterDtoIsInvalid()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "",
            Password = "",
            Username = ""
        };
        
        // Act
        var action = async () => await _userService.Register(registerDto);
        
        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Login_ShouldReturnUserId_WhenLoginDtoIsValid()
    {
        // Arrange
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);
        
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };
        
        // Act
        var result = await _userService.Login(loginDto);
        
        // Assert
        result.Should().Be(userId);
    }
    
    [Fact]
    public async Task Login_ShouldThrowValidationException_WhenLoginDtoIsInvalid()
    {
        // Arrange
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        await _userService.Register(registerDto);
        
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = ""
        };
        
        // Act
        var action = async () => await _userService.Login(loginDto);
        
        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetUsersLeaderbord_ShouldReturnListOfUsers()
    {
        // Arrange
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        
        // Act
        await _userService.Register(registerDto);
        var leaderboard = await _userService.GetUsersLeaderboard();

        // Assert
        leaderboard.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);
        
        // Act
        var user = await _userService.GetUser(userId);

        // Assert
        user.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetUserByUsername_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        await _userService.Register(registerDto);
        
        // Act
        var user = await _userService.GetUser(registerDto.Username);

        // Assert
        user.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetAnimeComments_ShouldReturnListOfCommentsForAnime_WhenAnimeIdIsValid()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        const string commentText = "This is a test comment for the anime.";
        await _userService.AddCommentToAnime(userId, animeId, commentText);

        // Act
        var comments = await _userService.GetAnimeComments(animeId);

        // Assert
        comments.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AddCommentToAnime_ShouldAddCommentToAnime_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        // Act
        const string commentText = "This is a test comment.";
        var commentId = await _userService.AddCommentToAnime(userId, animeId, commentText);

        // Assert
        commentId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task DeleteComment_ShouldDeleteComment_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        const string commentText = "This is a test comment.";
        var commentId = await _userService.AddCommentToAnime(userId, animeId, commentText);

        // Act
        await _userService.DeleteComment(userId, commentId);

        // Assert
        var deletedComment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        deletedComment.Should().BeNull();
    }
    
    [Fact]
    public async Task AddAnimeToWatchingList_ShouldAddAnimeToWatchingList_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;
        const int currentEpisode = 5;
        const float secondsTotal = 3600; // 1 hour

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);
        
        // Act
        await _userService.AddAnimeToWatchingList(userId, animeId, currentEpisode, secondsTotal);

        // Assert
        var user = await _userService.GetUser(userId);
        user.CurrentlyWatchingAnime.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task FinishEpisode_ShouldFinishEpisodeAndUpdateLists_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1; 
        const int episodeFinished = 26; // All episodes watched

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        await _userService.AddAnimeToWatchingList(userId, animeId, episodeFinished, 3600);
        
        // Act
        await _userService.FinishEpisode(userId, animeId, episodeFinished);

        // Assert
        var user = await _userService.GetUser(userId);
        user.CurrentlyWatchingAnime.Should().BeEmpty();
        user.WatchedAnime.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateTimestamps_ShouldUpdateTimestamps_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;
        const int secondsWatched = 1800; // 30 minutes

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        await _userService.AddAnimeToWatchingList(userId, animeId, 1, 3600);

        // Act
        await _userService.UpdateTimestamps(userId, animeId, secondsWatched);

        // Assert
        var user = await _userService.GetUser(userId);
        user.CurrentlyWatchingAnime.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RemoveAnimeFromWatchingList_ShouldRemoveAnimeFromWatchingList_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        await _userService.AddAnimeToWatchingList(userId, animeId, 1, 3600);
        
        // Act
        await _userService.RemoveAnimeFromWatchingList(userId, animeId);

        // Assert
        var user = await _userService.GetUser(userId);
        user.CurrentlyWatchingAnime.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAnimeToWatchedList_ShouldAddAnimeToWatchedList_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        // Act
        await _userService.AddAnimeToWatchedList(userId, animeId);

        // Assert
        var user = await _userService.GetUser(userId);
        var animeInWatchedList = user.WatchedAnime.FirstOrDefault(x => x.AnimeId == animeId);
        animeInWatchedList.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAnimeToWishlist_ShouldAddAnimeToWishlist_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        // Act
        await _userService.AddAnimeToWishlist(userId, animeId);

        // Assert
        var user = await _userService.GetUser(userId);
        var animeInWishlist = user.Wishlist.FirstOrDefault(x => x.AnimeId == animeId);
        animeInWishlist.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveAnimeFromWishlist_ShouldRemoveAnimeFromWishlist_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        await _userService.AddAnimeToWishlist(userId, animeId);

        // Act
        await _userService.RemoveAnimeFromWishlist(userId, animeId);

        // Assert
        var user = await _userService.GetUser(userId);
        user.Wishlist.Should().BeEmpty();
    }
    
    [Fact]
    public async Task AddAnimeToTierlist_ShouldAddAnimeToTierlist_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);

        // Act
        await _userService.AddAnimeToTierlist(userId, animeId);

        // Assert
        var user = await _userService.GetUser(userId);
        var animeInTierlist = user.Tierlist.FirstOrDefault(x => x.AnimeId == animeId);
        animeInTierlist.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangeTierlistOrder_ShouldChangeAnimeOrderInTierlist_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;
        const int newPosition = 2; 

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);
        await _userService.AddAnimeToTierlist(userId, animeId);

        // Act
        await _userService.ChangeTierlistOrder(userId, animeId, newPosition);

        // Assert
        var user = await _userService.GetUser(userId);
        var animeInTierlist = user.Tierlist.FirstOrDefault(x => x.AnimeId == animeId);
        animeInTierlist.Should().NotBeNull();
        animeInTierlist!.Position.Should().Be(newPosition);
    }

    [Fact]
    public async Task RemoveAnimeFromTierlist_ShouldRemoveAnimeFromTierlist_WhenValidParametersProvided()
    {
        // Arrange
        const long animeId = 1;

        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Email, f => f.Person.Email);
        var registerDto = faker.Generate();
        var userId = await _userService.Register(registerDto);
        await _userService.AddAnimeToTierlist(userId, animeId);

        // Act
        await _userService.RemoveAnimeFromTierlist(userId, animeId);

        // Assert
        var user = await _userService.GetUser(userId);
        var animeInTierlist = user.Tierlist.FirstOrDefault(x => x.AnimeId == animeId);
        animeInTierlist.Should().BeNull();
    }
}