using Anizavr.Backend.Application.Configuration;
using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.Application.Shared;
using Anizavr.Backend.Application.ShikimoriApi;
using Anizavr.Backend.Domain.Entities.Kodik;
using Anizavr.Backend.Domain.Entities.Shikimori;
using Anizavr.Backend.Domain.Exceptions;
using Mapster;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;
using Anime = Anizavr.Backend.Domain.Entities.Anime;

namespace Anizavr.Backend.Application.Services;

public class AnimeService : IAnimeService
{
    private readonly IShikimoriClient _shikimoriClient;
    private readonly IKodikApi _kodikApi;
    private readonly IShikimoriApi _shikimoriApi;
    private readonly IApplicationConfiguration _configuration;

    public AnimeService(IShikimoriClient shikimoriClient, 
        IKodikApi kodikApi, 
        IShikimoriApi shikimoriApi,
        IApplicationConfiguration configuration)
    {
        _shikimoriClient = shikimoriClient;
        _kodikApi = kodikApi;
        _shikimoriApi = shikimoriApi;
        _configuration = configuration;
    }

    #region Anime info

    public async Task<Anime> GetAnimeById(long id)
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
        
        var kodikDetails = await _kodikApi.GetAnime(id, _configuration.KodikKey);
        if (id == AnimeHelper.DeathNoteId)
        {
            AnimeHelper.FixDeathNotePoster(shikimoriDetails);
        }

        var anime = new Anime
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
        if (id == AnimeHelper.DeathNoteId)
        {
            return AnimeHelper.AnimeSimilarToDeathNote;
        }

        var similar = await _shikimoriClient.GetSimilar(id);

        AnimeHelper.FixDeathNotePoster(similar.FirstOrDefault(x => x.Id == AnimeHelper.DeathNoteId));
        return similar.Adapt<List<AnimePreview>>();
    }

    #endregion

    #region Search 
    
    public async Task<KodikResults> SearchAnime(string query, string? genres = null)
    {
        KodikResults search;
        if (genres is null)
        {
            search = await _kodikApi.SearchAnime(query, _configuration.KodikKey);
        }
        else
        {
            search = await _kodikApi.SearchAnime(query, genres);
        }

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

    #region Anime lists
    
    public async Task<List<AnimePreview>> GetPopularAnime(int limit, int page)
    {
        var popularAnime = await _shikimoriClient.GetAnime(new AnimeRequestSettings
        {
            order = Order.popularity,
            limit = limit,
            page = page,
            kind = "tv,movie"
        });

        AnimeHelper.FixDeathNotePoster(popularAnime.FirstOrDefault(x => x.Id == AnimeHelper.DeathNoteId));

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
    
    #endregion
}