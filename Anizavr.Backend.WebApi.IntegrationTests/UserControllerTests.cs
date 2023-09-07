using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.Validators;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Anizavr.Backend.WebApi.IntegrationTests;

public class UserControllerTests
{
    private readonly HttpClient _client;
    private readonly IAnizavrDbContext _dbContext;
    private readonly IUserService _userService;
    
    public UserControllerTests()
    {
        var contextOptions = new DbContextOptionsBuilder<AnizavrDbContext>()
            .UseInMemoryDatabase("AnizavrDb")
            .Options;

        _dbContext = new AnizavrDbContext(contextOptions);

        var animeService = Substitute.For<IAnimeService>();
        var registerDtoValidator = new RegisterDtoValidatior();
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
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registerDto);
        var jwtToken = await response.Content.ReadAsStringAsync();
        var deserializedToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        jwtToken.Should().NotBeEmpty();
        deserializedToken.Issuer.Should().Be("https://anizavr.tech");
        deserializedToken.Audiences.Should().Contain("https://anizavr.tech");
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
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
        
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
        var faker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var registerDto = faker.Generate();
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
        deserializedToken.Issuer.Should().Be("https://anizavr.tech");
        deserializedToken.Audiences.Should().Contain("https://anizavr.tech");
        deserializedToken.Claims.Should().Contain(x => x.Type == "UserId");
    }

    [Fact]
    public async Task Login_EndpointReturnsValidationException_WhenLoginDtoIsInvalid()
    {
        // Arrange
        var loginDto = new LoginDto()
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
        var faker = new Faker<LoginDto>()
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        var loginDto = faker.Generate();
        
        // Act
        _dbContext.Users.RemoveRange(_dbContext.Users);
        var response = await _client.PostAsJsonAsync("/api/users/login", loginDto);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetUser_EndpointReturnsUser_WhenUserExists()
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
}