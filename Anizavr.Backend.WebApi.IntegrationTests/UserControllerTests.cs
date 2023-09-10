using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.Validators;
using Anizavr.Backend.WebApi.IntegrationTests.Helpers;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ShikimoriSharp.Classes;
using Xunit;

namespace Anizavr.Backend.WebApi.IntegrationTests;

public class UserControllerTests
{
    private readonly HttpClient _client;
    private readonly IAnizavrDbContext _dbContext;
    private readonly IUserService _userService;
    
    private readonly AnimeID _testAnime;
    private readonly Faker<LoginDto> _loginDtoFaker;
    private readonly Faker<RegisterDto> _registerDtoFaker;

    public UserControllerTests()
    {
        var authFakerFactory = new AuthFakerFactory();
        _loginDtoFaker = authFakerFactory.GetLoginDtoFaker();
        _registerDtoFaker = authFakerFactory.GetRegisterDtoFaker();
        
        _testAnime = new TestAnimeFactory().GetTestAnime();
        
        var contextOptions = new DbContextOptionsBuilder<AnizavrDbContext>()
            .UseInMemoryDatabase("AnizavrDb")
            .Options;
        _dbContext = new AnizavrDbContext(contextOptions);

        var animeService = Substitute.For<IAnimeService>();
        animeService
            .GetShikimoriAnimeById(Arg.Any<long>())
            .Returns(Task.FromResult(_testAnime));
        
        var registerDtoValidator = new RegisterDtoValidator();
        var loginDtoValidator = new LoginDtoValidator();
        
        _userService = new UserService(_dbContext, animeService, registerDtoValidator, loginDtoValidator);

        var factory = new WebApplicationFactory<Program>();
        _client = factory.WithWebHostBuilder(builder =>
        {            
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<IUserService>(_ => _userService);
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task Register_EndpointReturnsUserId_WhenRegisterDtoIsValid()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await response.Content.ReadAsStringAsync();
        var deserializedToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        jwtToken.Should().NotBeEmpty();
        deserializedToken.Claims.Should().Contain(x => x.Type == "UserId");
        deserializedToken.Payload
            .Where(x => x.Key == "UserId")
            .Select(x => Guid.Parse(x.Value.ToString()!))
            .First()
            .Should().NotBe(Guid.Empty);
    }
    
    [Fact]
    public async Task Register_EndpointReturnsValidationException_WhenRegisterDtoIsInvalid()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "",
            Password = "",
            Username = ""
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Register_EndpointReturnsException_WhenUserWithSameDataAlreadyExists()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        await _client.PostAsJsonAsync("/api/users/register", registerDto);
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task Login_EndpointReturnsUserId_WhenLoginDtoIsValid()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        await _client.PostAsJsonAsync("/api/users/register", registerDto);
        
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginDto);
        var jwtToken = await response.Content.ReadAsStringAsync();
        var deserializedToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        jwtToken.Should().NotBeEmpty();
        deserializedToken.Claims.Should().Contain(x => x.Type == "UserId");
    }

    [Fact]
    public async Task Login_EndpointReturnsValidationException_WhenLoginDtoIsInvalid()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "",
            Password = ""
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginDto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_EndpointReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var loginDto = _loginDtoFaker.Generate();
        _dbContext.Users.RemoveRange(_dbContext.Users);
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginDto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetUser_EndpointReturnsUser_WhenUserExists()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var userId = await _userService.Register(registerDto);
        
        // Act
        var response = await _client.GetAsync($"/api/users/getUser/{registerDto.Username}");
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        user.Should().NotBeNull();
        user!.Id.Should().Be(userId);
        user.Username.Should().Be(registerDto.Username);
    }
    
    [Fact]
    public async Task GetUser_EndpointReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        const string username = "notExistingUsername";
        
        // Act
        var response = await _client.GetAsync($"/api/users/getUser/{username}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetCurrentUser_EndpointReturnsCurrentUser()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var response = await _client.GetAsync("/api/users/getCurrentUser");
        var user = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        user.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetUsersLeaderboard_EndpointReturnsLeaderboard()
    {
        // Arrange
        var registerDtos = _registerDtoFaker.Generate(10);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        foreach (var registerDto in registerDtos)
        {
            await _userService.Register(registerDto);
        }
        
        // Act
        var response = await _client.GetAsync("/api/users/getUsersLeaderbord");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        users.Should().NotBeNull();
        users!.Count.Should().Be(registerDtos.Count);
    }
    
    [Fact]
    public async Task GetComments_EndpointReturnsComments()
    {
        // Arrange
        var addCommentDto = new AddCommentDto
        {
            AnimeId = _testAnime.Id,
            Text = "This is a test comment."
        };
        await _client.PostAsJsonAsync("/api/users/addComment", addCommentDto);

        // Act
        var response = await _client.GetAsync($"/api/users/getComments/{_testAnime.Id}");
        var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        comments.Should().NotBeNull();
    }
    
    [Fact]
    public async Task AddComment_EndpointAddsComment()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();
        
        var addCommentDto = new AddCommentDto
        {
            AnimeId = _testAnime.Id,
            Text = "This is a test comment."
        };

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var response = await _client.PostAsJsonAsync("/api/users/addComment", addCommentDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteComment_EndpointDeletesComment()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        var addCommentDto = new AddCommentDto
        {
            AnimeId = _testAnime.Id,
            Text = "This is a test comment."
        };

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        await _client.PostAsJsonAsync("/api/users/addComment", addCommentDto);

        var userId = new JwtSecurityTokenHandler()
            .ReadJwtToken(jwtToken)
            .Payload
            .Where(x => x.Key == "UserId")
            .Select(x => Guid.Parse(x.Value.ToString()!))
            .First();
        
        var createdCommentFromDb = await _dbContext.Comments
            .Include(x=>x.Author)
            .FirstAsync(x => x.Author.Id == userId);

        // Act
        var response = await _client.DeleteAsync($"/api/users/deleteComment/{createdCommentFromDb.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task AddAnimeToWatchedList_EndpointAddsAnimeToWatchedList()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var response = await _client.PostAsync($"/api/users/addToWatched/{_testAnime.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddAnimeToWatchingList_EndpointAddsAnimeToWatchingList()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        const int currentEpisode = 5;
        const float secondsTotal = 3600; 

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var response = await _client.PostAsync($"/api/users/addToWatching/{_testAnime.Id}" +
                                               $"?currentEpisode={currentEpisode}" +
                                               $"&secondsTotal={secondsTotal}", 
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddAnimeToWishlist_EndpointAddsAnimeToWishlist()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var response = await _client.PostAsync($"/api/users/addToWishlist/{_testAnime.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FinishEpisode_EndpointFinishesEpisode()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        var currentEpisode = 1; 
        var secondsTotal = 0; 

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        await _client.PostAsync($"/api/users/addToWatching/{_testAnime.Id}?currentEpisode={currentEpisode}&secondsTotal={secondsTotal}", null);

        var episodeFinished = 5;

        // Act
        var response = await _client.PutAsync($"/api/users/finishEpisode?animeId={_testAnime.Id}&episodeFinished={episodeFinished}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTimestamps_EndpointUpdatesTimestamps()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        var currentEpisode = 1; 
        var secondsTotal = 0;

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        await _client.PostAsync($"/api/users/addToWatching/{_testAnime.Id}?currentEpisode={currentEpisode}&secondsTotal={secondsTotal}", null);

        var secondsWatched = 3600;

        // Act
        var response = await _client.PutAsync($"/api/users/updateTimestamps?animeId={_testAnime.Id}&secondsWatched={secondsWatched}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    [Fact]
    public async Task RemoveAnimeFromWatchingList_EndpointRemovesAnimeFromWatchingList()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        var currentEpisode = 1;
        var secondsTotal = 0; 

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        await _client.PostAsync($"/api/users/addToWatching/{_testAnime.Id}?currentEpisode={currentEpisode}&secondsTotal={secondsTotal}", null);

        // Act
        var response = await _client.DeleteAsync($"/api/users/removeFromWatching?animeId={_testAnime.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveAnimeFromWishlist_EndpointRemovesAnimeFromWishlist()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();
        
        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        await _client.PostAsync($"/api/users/addToWishlist/{_testAnime.Id}", null);

        // Act
        var response = await _client.DeleteAsync($"/api/users/removeFromWishlist?animeId={_testAnime.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task AddAnimeToTierlist_EndpointAddsAnimeToTierlist()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();
        
        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var response = await _client.PostAsync($"/api/users/addToTierlist/{_testAnime.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangeTierlistOrder_EndpointChangesTierlistOrder()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();
        
        // Act
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var newPosition = 2;
        var response = await _client.PutAsync($"/api/users/changeTierlistOrder/{_testAnime.Id}?newPosition={newPosition}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveAnimeFromTierlist_EndpointRemovesAnimeFromTierlist()
    {
        // Arrange
        var registerDto = _registerDtoFaker.Generate();
        var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await registerResponse.Content.ReadAsStringAsync();

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
        var addToTierlistResponse = await _client.PostAsync($"/api/users/addToTierlist/{_testAnime.Id}", null);
        addToTierlistResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _client.DeleteAsync($"/api/users/removeFromTierlist/{_testAnime.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}