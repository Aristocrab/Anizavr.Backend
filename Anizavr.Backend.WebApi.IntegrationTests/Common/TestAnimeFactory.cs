using AutoFixture;
using ShikimoriSharp.Classes;

namespace Anizavr.Backend.WebApi.IntegrationTests.Common;

public class TestAnimeFactory
{
    private readonly AnimeID _anime;

    public TestAnimeFactory()
    {
        var fixture = new Fixture();
        _anime = fixture.Create<AnimeID>();
        _anime.Id = 1;
        _anime.Episodes = 26;
    }
    
    public AnimeID GetTestAnime()
    {
        return _anime;
    }
}