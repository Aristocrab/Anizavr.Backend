﻿using Application.KodikApi.Entities;
using Application.Services;
using Application.ShikimoriApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Shared;
using Anime = Application.Entities.Anime;

namespace WebApi.Controllers;

[AllowAnonymous]
[Route("/api")]
[ResponseCache(CacheProfileName = "Default")]
public class AnimeController : BaseController
{
    private readonly AnimeService _animeService;

    public AnimeController(AnimeService animeService)
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
        return AnimeService.GetGenres();
    }
}