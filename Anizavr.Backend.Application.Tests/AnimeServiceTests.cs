namespace Anizavr.Backend.Application.Tests;

public class AnimeServiceTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly IShikimoriClient _shikimoriClient;
    private readonly IKodikApi _kodikApi;
    private readonly IShikimoriApi _shikimoriApi;
    private readonly IAnimeService _animeService;

    public AnimeServiceTests()
    {
        _shikimoriClient = Substitute.For<IShikimoriClient>();
        _kodikApi = Substitute.For<IKodikApi>();
        _shikimoriApi = Substitute.For<IShikimoriApi>();
        _animeService = new AnimeService(_shikimoriClient, _kodikApi, _shikimoriApi);
    }

    [Fact]
    public async Task GetAnimeById_ShouldReturnAnime_WhenAnimeExists()
    {
        // Arrange
        const long animeId = 1;
        
        var shikimoriDetails = _fixture.Create<AnimeID>();
        shikimoriDetails.Id = animeId;
        _shikimoriClient.GetAnime(animeId).Returns(shikimoriDetails);

        var kodikResults = _fixture.Create<KodikResults>();
        _kodikApi.GetAnime(animeId, Arg.Any<string>()).Returns(kodikResults);

        // Act
        var result = await _animeService.GetAnimeById(animeId);

        // Assert
        result.ShikimoriDetails.Should().BeEquivalentTo(shikimoriDetails);
        result.KodikDetails.Should().BeEquivalentTo(kodikResults);
    }
    
    [Fact]
    public async Task GetShikimoriAnimeById_ShouldReturnAnime_WhenAnimeExists()
    {
        // Arrange
        const long animeId = 1;
        var shikimoriDetails = _fixture.Create<AnimeID>();
        shikimoriDetails.Id = animeId;

        _shikimoriClient.GetAnime(animeId).Returns(shikimoriDetails);

        // Act
        var result = await _animeService.GetShikimoriAnimeById(animeId);

        // Assert
        result.Should().BeEquivalentTo(shikimoriDetails);
    }
    
    [Fact]
    public async Task GetRelated_ShouldReturnRelatedAnime_WhenAnimeExists()
    {
        // Arrange
        const long animeId = 1;
        var relatedAnime = _fixture
            .CreateMany<Related>()
            .Select(x =>
            {
                x.Manga = null; 
                return x;
            })
            .ToArray();

        _shikimoriClient.GetRelated(animeId).Returns(relatedAnime);

        // Act
        var result = await _animeService.GetRelated(animeId);

        // Assert
        result.Should().BeEquivalentTo(relatedAnime.Adapt<ShikimoriRelated[]>());
    }

    [Fact]
    public async Task GetSimilarAnime_ShouldReturnSimilarAnime_WhenAnimeExists()
    {
        // Arrange
        const long animeId = 1;
        var similarAnime = _fixture.CreateMany<Anime>().ToArray();
        _shikimoriClient.GetSimilar(animeId).Returns(similarAnime);

        // Act
        var result = await _animeService.GetSimilarAnime(animeId);

        // Assert
        result.Should().BeEquivalentTo(similarAnime);
    }
    
    [Fact]
    public async Task SearchAnime_ShouldReturnSearchResults_WhenQueryIsValid()
    {
        // Arrange
        var query = "Attack on Titan";
        var kodikResults = _fixture.Create<KodikResults>();
        
        _kodikApi.SearchAnime(query, "").ReturnsForAnyArgs(kodikResults);

        // Act
        var result = await _animeService.SearchAnime(query);

        // Assert
        result.Should().BeEquivalentTo(kodikResults);
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(5, 2)]
    public async Task GetPopularAnime_ShouldReturnPopularAnime_WhenLimitAndPageAreValid(int limit, int page)
    {
        // Arrange
        var popularAnime = _fixture.CreateMany<Anime>().ToArray();

        _shikimoriClient.GetAnime(Arg.Any<AnimeRequestSettings>()).Returns(popularAnime);

        // Act
        var result = await _animeService.GetPopularAnime(limit, page);

        // Assert
        result.Should().BeEquivalentTo(popularAnime);
    }
    
    [Theory]
    [InlineData(10, 1)]
    [InlineData(5, 2)]
    public async Task GetTrendingAnime_ShouldReturnTrendingAnime_WhenLimitAndPageAreValid(int limit, int page)
    {
        // Arrange
        var trendingAnime = _fixture.CreateMany<Anime>().ToArray();

        _shikimoriApi
            .GetAnime(Arg.Any<Order>(), 
                Arg.Any<string>(), 
                Arg.Any<int>(), 
                Arg.Any<int>(), 
                Arg.Any<string>(), 
                Arg.Any<string>())
            .Returns(trendingAnime);

        // Act
        var result = await _animeService.GetTrendingAnime(limit, page);

        // Assert
        result.Should().BeEquivalentTo(trendingAnime);
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(5, 2)]
    public async Task GetJustReleasedAnime_ShouldReturnJustReleasedAnime_WhenLimitAndPageAreValid(int limit, int page)
    {
        // Arrange
        var justReleasedAnime = _fixture.CreateMany<Anime>().ToArray();

        _shikimoriApi
            .GetAnime(Arg.Any<Order>(), 
                Arg.Any<string>(), 
                Arg.Any<int>(), 
                Arg.Any<int>(), 
                Arg.Any<string>(), 
                Arg.Any<string>())
            .Returns(justReleasedAnime);

        // Act
        var result = await _animeService.GetJustReleasedAnime(limit, page);

        // Assert
        result.Should().BeEquivalentTo(justReleasedAnime);
    }
}