using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace Anizavr.Backend.Application.ShikimoriApi;

public interface IShikimoriClient
{
    Task<Anime[]> GetAnime(AnimeRequestSettings? settings = null, AccessToken? personalInformation = null);
    Task<AnimeID> GetAnime(long id, AccessToken? personalInformation = null);
    Task<Anime[]> GetAnime(Order order, string status, int limit, int page, string kind, string season);
    Task<Anime[]> GetSimilar(long id, AccessToken? personalInformation = null);
    Task<Related[]> GetRelated(long id, AccessToken? personalInformation = null);
}