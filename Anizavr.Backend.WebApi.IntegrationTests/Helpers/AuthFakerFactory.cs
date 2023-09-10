using Anizavr.Backend.Application.Dtos;
using Bogus;

namespace Anizavr.Backend.WebApi.IntegrationTests.Helpers;

public class AuthFakerFactory
{
    public AuthFakerFactory()
    {
        LoginDtoFaker = new Faker<LoginDto>()
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
        
        RegisterDtoFaker = new Faker<RegisterDto>()
            .RuleFor(x => x.Username, 
                f => f.Random.String2(6, 12))
            .RuleFor(x => x.Password, 
                f => f.Internet.Password())
            .RuleFor(x => x.Email, 
                f => f.Person.Email);
    }
    
    public Faker<LoginDto> LoginDtoFaker { get; }

    public Faker<RegisterDto> RegisterDtoFaker { get; }
}
