using Anizavr.Backend.Application.Common;
using Anizavr.Backend.Application.Dtos;
using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.Application.KodikApi.Entities;
using Anizavr.Backend.Application.ShikimoriApi;
using Anizavr.Backend.Application.ShikimoriApi.Entities;
using Anizavr.Backend.Domain.Exceptions;
using Mapster;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace Anizavr.Backend.Application.Services;

public class AnimeService : IAnimeService
{
    private readonly IShikimoriClient _shikimoriClient;
    private readonly IKodikClient _kodikClient;

    public AnimeService(IShikimoriClient shikimoriClient, IKodikClient kodikClient)
    {
        _shikimoriClient = shikimoriClient;
        _kodikClient = kodikClient;
    }

    #region AnimeDto info

    public async Task<AnimeDto> GetAnimeById(long id)
    {
        AnimeID shikimoriDetails;
        try
        {
            shikimoriDetails = await _shikimoriClient.GetAnime(id);
        }
        catch
        {
            throw new NotFoundException("Аниме", nameof(id), id.ToString());
        }
        
        var kodikDetails = await _kodikClient.GetAnime(id);
        if (id == DeathNoteFixHelper.DeathNoteId)
        {
            DeathNoteFixHelper.FixDeathNotePoster(shikimoriDetails);
        }

        var anime = new AnimeDto
        {
            ShikimoriDetails = shikimoriDetails,
            KodikDetails = kodikDetails
        };
        
        return anime;
    }
    
    public async Task<AnimeID> GetShikimoriAnimeById(long id)
    {
        AnimeID shikimoriDetails;
        try
        {
            shikimoriDetails = await _shikimoriClient.GetAnime(id);
        }
        catch
        {
            throw new NotFoundException("Аниме", nameof(id), id.ToString());
        }
        
        return shikimoriDetails;
    }

    public async Task<ShikimoriRelated[]> GetRelated(long id)
    {
        var shikimoriDetails = await _shikimoriClient.GetRelated(id);
        
        return shikimoriDetails
            .Where(x => x.Manga is null)
            .Adapt<ShikimoriRelated[]>();
    }

    public async Task<List<AnimePreview>> GetSimilarAnime(long id)
    {
        if (id == DeathNoteFixHelper.DeathNoteId)
        {
            return DeathNoteFixHelper.AnimeSimilarToDeathNote;
        }

        var similar = await _shikimoriClient.GetSimilar(id);

        DeathNoteFixHelper.FixDeathNotePoster(similar.FirstOrDefault(x => x.Id == DeathNoteFixHelper.DeathNoteId));
        return similar.Adapt<List<AnimePreview>>();
    }

    #endregion

    #region Search 
    
    public async Task<KodikResults> SearchAnime(string query, string? genres = null)
    {
        var search = await _kodikClient.SearchAnime(query);

        // Взять по одному переводу с каждого аниме
        var distinctResults = search.Results
            .Where(x => x.Shikimori_Id is not null)
            .DistinctBy(x => x.Shikimori_Id)
            .OrderByDescending(x => x.Material_Data?.Shikimori_rating)
            .ToList();
        
        search.Results = distinctResults;

        foreach (var result in search.Results)
        {
            // Костыль, чтоб постеры брались с Шикимори
            result.Material_Data!.Poster_Url = $"/system/animes/original/{result.Shikimori_Id}.jpg?1674378220";
        }
        
        return search;
    }
    
    public List<string> GetGenresList()
    {
        return ShikimoriGenres.List;
    }
    
    #endregion

    #region AnimeDto lists
    
    public async Task<List<AnimePreview>> GetPopularAnime(int limit, int page)
    {
        var popularAnime = await _shikimoriClient.GetAnime(new AnimeRequestSettings
        {
            order = Order.popularity,
            limit = limit,
            page = page,
            kind = "tv,movie"
        });

        DeathNoteFixHelper.FixDeathNotePoster(popularAnime.FirstOrDefault(x => x.Id == DeathNoteFixHelper.DeathNoteId));

        return popularAnime.Select(x => x.Adapt<AnimePreview>()).ToList();
    }
    
    public async Task<List<AnimePreview>> GetTrendingAnime(int limit, int page)
    {
        var now = DateTime.Now;
        var yearAgo = DateTime.Now.AddYears(-1);

        var trendingAnime = await _shikimoriClient.GetAnime(
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
        
        var trendingAnime = await _shikimoriClient.GetAnime(
            order: Order.popularity,
            status: "released",
            limit: limit,
            page: page,
            kind: "tv,movie",
            season: $"{now.Year},{yearAgo.Year}"
        );

        return trendingAnime.Select(x => x.Adapt<AnimePreview>()).ToList();
    }
    
    #endregion
}