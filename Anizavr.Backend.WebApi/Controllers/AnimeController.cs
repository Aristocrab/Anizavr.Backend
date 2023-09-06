using Anizavr.Backend.Application.Entities;
using Anizavr.Backend.Application.KodikApi.Entities;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.ShikimoriApi;
using Anizavr.Backend.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Controllers;

[AllowAnonymous]
[Route("/api")]
[ResponseCache(CacheProfileName = "DefaultCacheProfile")]
public class AnimeController : BaseController
{
    private readonly IAnimeService _animeService;

    public AnimeController(IAnimeService animeService)
    {
        _animeService = animeService;
    }

    [HttpGet("getAnime/{id}")]
    public async Task<Anime> GetAnimeById(int id)
    {
        return await _animeService.GetAnimeById(id);
    }

    [HttpGet("getSimilarAnime/{id}")]
    public async Task<List<AnimePreview>> GetSimilarAnime(int id)
    {
        return await _animeService.GetSimilarAnime(id);
    }

    [HttpGet("getRelatedAnime/{id}")]
    public async Task<ShikimoriRelated[]> GetRelated(int id)
    {
        return await _animeService.GetRelated(id);
    }

    [HttpGet("searchAnime")]
    public async Task<KodikResults> SearchAnime(string query, string? genres = null)
    {
        return await _animeService.SearchAnime(query, genres);
    }

    [HttpGet("getPopularAnime")]
    public async Task<List<AnimePreview>> GetPopularAnime(int limit = 5, int page = 1)
    {
        return await _animeService.GetPopularAnime(limit, page);
    }

    [HttpGet("getTrendingAnime")]
    public async Task<List<AnimePreview>> GetOngoingAnime(int limit = 5, int page = 1)
    {
        return await _animeService.GetTrendingAnime(limit, page);
    }

    [HttpGet("getJustReleasedAnime")]
    public async Task<List<AnimePreview>> GetJustReleasedAnime(int limit = 5, int page = 1)
    {
        return await _animeService.GetJustReleasedAnime(limit, page);
    }
    
    [HttpGet("getGenres")]
    public List<string> GetGenres()
    {
        return _animeService.GetGenresList();
    }
}