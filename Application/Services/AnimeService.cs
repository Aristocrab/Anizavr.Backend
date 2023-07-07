using Application.AnimeSkipApi;
using Application.AnimeSkipApi.Entities;
using Application.Entities;
using Application.Exceptions;
using Application.KodikApi;
using Application.KodikApi.Entities;
using Application.ShikimoriApi;
using FuzzySharp;
using Mapster;
using ShikimoriSharp;
using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;
using Anime = Application.Entities.Anime;

namespace Application.Services;

public class AnimeService
{
    private readonly ShikimoriClient _shikimoriClient;
    private readonly IKodikApi _kodikApi;
    private readonly IShikimoriApi _shikimoriApi;
    private readonly AnimeSkipService _animeSkipService;

    public AnimeService(ShikimoriClient shikimoriClient, 
        IKodikApi kodikApi, 
        IShikimoriApi shikimoriApi, 
        AnimeSkipService animeSkipService)
    {
        _shikimoriClient = shikimoriClient;
        _kodikApi = kodikApi;
        _shikimoriApi = shikimoriApi;
        _animeSkipService = animeSkipService;
    }

    public async Task<Anime> GetAnimeById(long id)
    {
        AnimeID shikimoriDetails;
        try
        {
            shikimoriDetails = await _shikimoriClient.Animes.GetAnime(id);
        }
        catch
        {
            throw new NotFoundException(nameof(id), id.ToString());
        }
        
        var kodikDetails = await _kodikApi.GetAnime(id);
        var timestamps = await GetTimestampsById(id);

        // Костыль для Тетради смерти
        if (id == 1535)
        {
            var imageId = shikimoriDetails.Image.Original.Split('?')[1];
            shikimoriDetails.Image.Original = $"/system/animes/original/1535.jpg?{imageId}";
        }
        
        var anime = new Anime
        {
            ShikimoriDetails = shikimoriDetails,
            KodikDetails = kodikDetails,
            Timestamps = timestamps?.FindShowsByExternalId.FirstOrDefault()
        };
        
        return anime;
    }

    private async Task<ShowsByExternalId?> GetTimestampsById(long id)
    {
        return await _animeSkipService.GetTimestampsById(id);
    }
    
    public async Task<Franchise> GetFranchise(long id)
    {
        var shikimoriDetails = await _shikimoriClient.Animes.GetFranchise(id);
        
        return shikimoriDetails;
    }
    
    public async Task<ShikimoriRelated[]> GetRelated(long id)
    {
        var shikimoriDetails = await _shikimoriClient.Animes.GetRelated(id);
        
        return shikimoriDetails
            .Where(x => x.Manga is null)
            .Adapt<ShikimoriRelated[]>();
    }

    public async Task<ShikimoriAnime[]> GetSimilarAnime(long id)
    {
        var similar = await _shikimoriClient.Animes.GetSimilar(id);
        
        return similar.Adapt<ShikimoriAnime[]>();
    }

    public async Task<KodikAnime> SearchAnime(string query)
    {
        var search = await _kodikApi.SearchAnime(query);

        // Взять по одному переводу с каждого аниме
        // Сортировка по совпадению текста + приоритет для "[ТВ-*]"
        // todo: спарсить JSON со всеми аниме и сделать нормальный поиск?
        var distinctResults = search.Results
            .DistinctBy(x => x.Shikimori_Id)
            .OrderByDescending(x => Fuzz.Ratio(query, x.Title) + (x.Title.Contains('[') ? 100 : 0)) 
            .ThenBy(x => x.Title)
            .ToList();
        
        search.Results = distinctResults;

        foreach (var result in search.Results)
        {
            // Костыль, чтоб постеры брались с Шикимори
            result.Material_Data!.Poster_Url = $"https://shikimori.me/system/animes/original/{result.Shikimori_Id}.jpg?1674378220";
        }
        
        return search;
        // var search = await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
        // {
        //     order = Order.popularity,
        //     kind = "tv,movie",
        //     search = query,
        //     limit = limit,
        // });
        //
        // var limitAnime = search.Take(limit);
        //
        // return limitAnime.Select(x => x.Adapt<AnimePreview>()).ToList();
    }

    public async Task<List<AnimePreview>> GetPopularAnime(int limit, int page)
    {
        var popularAnime = await _shikimoriClient.Animes.GetAnime(new AnimeRequestSettings
        {
            order = Order.popularity,
            limit = limit,
            page = page,
            kind = "tv,movie"
        });

        // Костыль для Тетради смерти
        if (popularAnime.Any(x => x.Id == 1535))
        {
            var deathNote = popularAnime.First(x => x.Id == 1535);
            var imageId = deathNote.Image.Original.Split('?')[1];
            deathNote.Image.Original = $"/system/animes/original/1535.jpg?{imageId}";
        }

        return popularAnime.Select(x => x.Adapt<AnimePreview>()).ToList();
    }
    
    public async Task<List<AnimePreview>> GetTrendingAnime(int limit, int page)
    {
        var now = DateTime.Now;
        var yearAgo = DateTime.Now.AddYears(-1);

        var trendingAnime = await _shikimoriApi.GetAnime(
            order: Order.popularity,
            status: "ongoing",
            limit: limit,
            page: page,
            kind: "tv,movie",
            season: $"{now.Year},{yearAgo.Year}"
            );

        return trendingAnime.Select(x => x.Adapt<AnimePreview>()).ToList();
    }
    
    public async Task<List<AnimePreview>> GetJustReleasedAnime(int limit, int page)
    {
        var now = DateTime.Now;
        var yearAgo = DateTime.Now.AddYears(-1);
        
        var trendingAnime = await _shikimoriApi.GetAnime(
            order: Order.popularity,
            status: "released",
            limit: limit,
            page: page,
            kind: "tv,movie",
            season: $"{now.Year},{yearAgo.Year}"
            );

        return trendingAnime.Select(x => x.Adapt<AnimePreview>()).ToList();
    }
}