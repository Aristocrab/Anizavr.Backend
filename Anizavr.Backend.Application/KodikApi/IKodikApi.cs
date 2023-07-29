using Anizavr.Backend.Application.KodikApi.Entities;
using Anizavr.Backend.Application.Shared;
using Refit;

namespace Anizavr.Backend.Application.KodikApi;

public interface IKodikApi
{
    [Get("/search?token={token}&shikimori_id={shikimoriId}&")]
    Task<KodikResults> GetAnime(long shikimoriId, string token = Constants.KodikKey);
    
    [Get("/search?token={token}&title={query}&types=anime-serial,anime&limit=100&with_material_data=true")]
    Task<KodikResults> SearchAnime(string query, string token = Constants.KodikKey);
    
    [Get("/search?token={token}&title={query}&types=anime-serial,anime&limit=100&with_material_data=true&anime_genres={genres}")]
    Task<KodikResults> SearchAnime(string query, string genres, string token = Constants.KodikKey);
}