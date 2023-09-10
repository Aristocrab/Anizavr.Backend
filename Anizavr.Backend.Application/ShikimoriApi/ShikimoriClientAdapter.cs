using ShikimoriSharp;
using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Information;
using ShikimoriSharp.Settings;

namespace Anizavr.Backend.Application.ShikimoriApi;

public class ShikimoriClientAdapter : IShikimoriClient
{
    private readonly IShikimoriApi _shikimoriApi;
    private readonly Animes _animes;

    public ShikimoriClientAdapter(ShikimoriClient shikimoriClient, IShikimoriApi shikimoriApi)
    {
        _shikimoriApi = shikimoriApi;
        _animes = new Animes(shikimoriClient.Client);
    }
    
    public Task<Anime[]> GetAnime(AnimeRequestSettings? settings = null, AccessToken? personalInformation = null)
    {
        return _animes.GetAnime(settings, personalInformation);
    }

    public Task<AnimeID> GetAnime(long id, AccessToken? personalInformation = null)
    {
        return _animes.GetAnime(id, personalInformation);
    }

    public Task<Anime[]> GetAnime(Order order, string status, int limit, int page, string kind, string season)
    {
        return _shikimoriApi.GetAnime(order, status, limit, page, kind, season);
    }

    public Task<Anime[]> GetSimilar(long id, AccessToken? personalInformation = null)
    {
        return _animes.GetSimilar(id, personalInformation);
    }

    public Task<Related[]> GetRelated(long id, AccessToken? personalInformation = null)
    {
        return _animes.GetRelated(id, personalInformation);
    }
}