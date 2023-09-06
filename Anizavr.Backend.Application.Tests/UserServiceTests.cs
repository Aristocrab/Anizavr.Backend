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
}