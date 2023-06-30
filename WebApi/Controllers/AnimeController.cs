using Application.Entities;
using Application.KodikApi.Entities;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShikimoriSharp.AdditionalRequests;
using WebApi.Controllers.Shared;
using Anime = Application.Entities.Anime;

namespace WebApi.Controllers;

[AllowAnonymous]
[Route("/api")]
public class AnimeController : BaseController
{
    private readonly AnimeService _animeService;

    public AnimeController(AnimeService animeService)
    {
        _animeService = animeService;
    }

    [HttpGet("getAnime/{id}")]
    [ResponseCache(VaryByQueryKeys = new[] { "id" }, Duration = 120)]
    public async Task<Anime> GetAnimeById(int id)
    {
        return await _animeService.GetAnimeById(id);
    }

    [HttpGet("getFranchise/{id}")]
    [ResponseCache(VaryByQueryKeys = new[] { "id" }, Duration = 120)]
    public async Task<Franchise> GetFranchise(int id)
    {
        return await _animeService.GetFranchise(id);
    }

    [HttpGet("getSimilarAnime/{id}")]
    [ResponseCache(VaryByQueryKeys = new[] { "id" }, Duration = 120)]
    public async Task<ShikimoriAnime[]> GetSimilarAnime(int id)
    {
        return await _animeService.GetSimilarAnime(id);
    }

    [HttpGet("getRelatedAnime/{id}")]
    [ResponseCache(VaryByQueryKeys = new[] { "id" }, Duration = 120)]
    public async Task<ShikimoriRelated[]> GetRelated(int id)
    {
        return await _animeService.GetRelated(id);
    }

    [HttpGet("searchAnime")]
    [ResponseCache(VaryByQueryKeys = new[] { "query" }, Duration = 120)]
    public async Task<KodikAnime> SearchAnime(string query)
    {
        return await _animeService.SearchAnime(query);
    }

    [HttpGet("getPopularAnime")]
    [ResponseCache(VaryByQueryKeys = new[] { "limit", "page" }, Duration = 120)]
    public async Task<List<AnimePreview>> GetPopularAnime(int limit = 5, int page = 1)
    {
        return await _animeService.GetPopularAnime(limit, page);
    }

    [HttpGet("getTrendingAnime")]
    [ResponseCache(VaryByQueryKeys = new[] { "limit", "page" }, Duration = 120)]
    public async Task<List<AnimePreview>> GetOngoingAnime(int limit = 5, int page = 1)
    {
        return await _animeService.GetTrendingAnime(limit, page);
    }

    [HttpGet("getJustReleasedAnime")]
    [ResponseCache(VaryByQueryKeys = new[] { "limit", "page" }, Duration = 120)]
    public async Task<List<AnimePreview>> GetJustReleasedAnime(int limit = 5, int page = 1)
    {
        return await _animeService.GetJustReleasedAnime(limit, page);
    }
}