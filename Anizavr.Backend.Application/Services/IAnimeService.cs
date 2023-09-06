using Anizavr.Backend.Application.KodikApi.Entities;
using Anizavr.Backend.Application.ShikimoriApi;
using ShikimoriSharp.Classes;
using Anime = Anizavr.Backend.Application.Entities.Anime;

namespace Anizavr.Backend.Application.Services;

public interface IAnimeService
{
    Task<Anime> GetAnimeById(long id);
    Task<AnimeID> GetShikimoriAnimeById(long id);
    Task<ShikimoriRelated[]> GetRelated(long id);
    Task<List<AnimePreview>> GetSimilarAnime(long id);
    Task<KodikResults> SearchAnime(string query, string? genres = null);
    List<string> GetGenresList();
    Task<List<AnimePreview>> GetPopularAnime(int limit, int page);
    Task<List<AnimePreview>> GetTrendingAnime(int limit, int page);
    Task<List<AnimePreview>> GetJustReleasedAnime(int limit, int page);
}