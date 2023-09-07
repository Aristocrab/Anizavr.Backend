using System.Net;
using System.Net.Http.Json;
using Anizavr.Backend.Application.Entities;
using Anizavr.Backend.Application.KodikApi.Entities;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.ShikimoriApi;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Anizavr.Backend.WebApi.IntegrationTests;

public class AnimeControllerTests
{
    private readonly IFixture _fixture;
    private readonly IAnimeService _animeService;
    private readonly HttpClient _client;
    
    public AnimeControllerTests()
    {
        _fixture = new Fixture();
        _animeService = Substitute.For<IAnimeService>();
        var factory = new WebApplicationFactory<Program>();
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<IAnimeService>(_ => _animeService);
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task GetAnimeById_EndpointReturnsAnime()
    {
        // Arrange
        var expectedAnime = _fixture.Create<Anime>();
        _animeService.GetAnimeById(Arg.Any<long>()).Returns(expectedAnime);

        // Act
        var response = await _client.GetAsync("/api/getAnime/1");
        var anime = await response.Content.ReadFromJsonAsync<Anime>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        anime.Should().NotBeNull();
        anime!.ShikimoriDetails.Id.Should().Be(expectedAnime.ShikimoriDetails.Id);
    }
    
    [Fact]
    public async Task GetSimilarAnime_EndpointReturnsAnimeList()
    {
        // Arrange
        var expectedAnimeList = _fixture.Create<List<AnimePreview>>();
        _animeService.GetSimilarAnime(Arg.Any<long>()).Returns(expectedAnimeList);

        // Act
        var response = await _client.GetAsync("/api/getSimilarAnime/1");
        var animeList = await response.Content.ReadFromJsonAsync<List<AnimePreview>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        animeList.Should().NotBeNull();
        animeList.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetRelatedAnime_EndpointReturnsAnimeList()
    {
        // Arrange
        var expectedAnimeList = _fixture.Create<ShikimoriRelated[]>();
        _animeService.GetRelated(Arg.Any<long>()).Returns(expectedAnimeList);

        // Act
        var response = await _client.GetAsync("/api/getRelatedAnime/1");
        var animeList = await response.Content.ReadFromJsonAsync<ShikimoriRelated[]>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        animeList.Should().NotBeNull();
        animeList.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task SearchAnime_EndpointReturnsAnimeList()
    {
        // Arrange
        var expectedAnimeList = _fixture.Create<KodikResults>();
        _animeService.SearchAnime(Arg.Any<string>(), Arg.Any<string>()).Returns(expectedAnimeList);

        // Act
        var response = await _client.GetAsync("/api/searchAnime?query=naruto");
        var animeList = await response.Content.ReadFromJsonAsync<KodikResults>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        animeList.Should().NotBeNull();
        animeList!.Results.Should().NotBeEmpty();
        animeList.Results.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetPopularAnime_EndpointReturnsAnimeList()
    {
        // Arrange
        var expectedAnimeList = _fixture.Create<List<AnimePreview>>();
        _animeService.GetPopularAnime(Arg.Any<int>(), Arg.Any<int>()).Returns(expectedAnimeList);

        // Act
        var response = await _client.GetAsync("/api/getPopularAnime");
        var animeList = await response.Content.ReadFromJsonAsync<List<AnimePreview>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        animeList.Should().NotBeNull();
        animeList.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetTrendingAnime_EndpointReturnsAnimeList()
    {
        // Arrange
        var expectedAnimeList = _fixture.Create<List<AnimePreview>>();
        _animeService.GetTrendingAnime(Arg.Any<int>(), Arg.Any<int>()).Returns(expectedAnimeList);

        // Act
        var response = await _client.GetAsync("/api/getTrendingAnime");
        var animeList = await response.Content.ReadFromJsonAsync<List<AnimePreview>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        animeList.Should().NotBeNull();
        animeList.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetJustReleasedAnime_EndpointReturnsAnimeList()
    {
        // Arrange
        var expectedAnimeList = _fixture.Create<List<AnimePreview>>();
        _animeService.GetJustReleasedAnime(Arg.Any<int>(), Arg.Any<int>()).Returns(expectedAnimeList);

        // Act
        var response = await _client.GetAsync("/api/getJustReleasedAnime");
        var animeList = await response.Content.ReadFromJsonAsync<List<AnimePreview>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        animeList.Should().NotBeNull();
        animeList.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetGenres_EndpointReturnsGenresList()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/getGenres");
        var genres = await response.Content.ReadFromJsonAsync<List<string>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        genres.Should().NotBeNull();
        genres.Should().NotBeEmpty();
    }
}