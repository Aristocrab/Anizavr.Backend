﻿using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.KodikApi.Entities;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.ShikimoriApi.Entities;
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
    
    [HttpGet("getAnime/{id:long}")]
    public Task<AnimeDto> GetAnimeById(long id)
    {
        return _animeService.GetAnimeById(id);
    }

    [HttpGet("getSimilarAnime/{id:long}")]
    public Task<List<AnimePreview>> GetSimilarAnime(long id)
    {
        return _animeService.GetSimilarAnime(id);
    }

    [HttpGet("getRelatedAnime/{id:long}")]
    public Task<ShikimoriRelated[]> GetRelated(long id)
    {
        return _animeService.GetRelated(id);
    }

    [HttpGet("searchAnime")]
    public Task<KodikResults> SearchAnime(string query, string? genres = null)
    {
        return _animeService.SearchAnime(query, genres);
    }

    [HttpGet("getPopularAnime")]
    public Task<List<AnimePreview>> GetPopularAnime(int limit = 5, int page = 1)
    {
        return _animeService.GetPopularAnime(limit, page);
    }

    [HttpGet("getTrendingAnime")]
    public Task<List<AnimePreview>> GetOngoingAnime(int limit = 5, int page = 1)
    {
        return _animeService.GetTrendingAnime(limit, page);
    }

    [HttpGet("getJustReleasedAnime")]
    public Task<List<AnimePreview>> GetJustReleasedAnime(int limit = 5, int page = 1)
    {
        return _animeService.GetJustReleasedAnime(limit, page);
    }
    
    [HttpGet("getGenres")]
    public List<string> GetGenres()
    {
        return _animeService.GetGenresList();
    }
}