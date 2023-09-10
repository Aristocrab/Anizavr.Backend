using Anizavr.Backend.Application.Dtos;
using Bogus;

namespace Anizavr.Backend.WebApi.IntegrationTests.Helpers;

public class AuthFakerFactory
{
    private readonly Faker<LoginDto> _loginDtoFaker;
    private readonly Faker<RegisterDto> _registerDtoFaker;

    public AuthFakerFactory()
    {
        _loginDtoFaker = new Faker<LoginDto>()
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        
        _registerDtoFaker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
    }

    public Faker<LoginDto> GetLoginDtoFaker()
    {
        return _loginDtoFaker;
    }
    
    public Faker<RegisterDto> GetRegisterDtoFaker()
    {
        return _registerDtoFaker;
    }
}
