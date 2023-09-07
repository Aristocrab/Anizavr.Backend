using Anizavr.Backend.Domain.Entities.Kodik;
using Refit;

namespace Anizavr.Backend.Application.KodikApi;

public interface IKodikApi
{
    [Get("/search?token={token}&shikimori_id={shikimoriId}&")]
    Task<KodikResults> GetAnime(long shikimoriId, string token);
    
    [Get("/search?token={token}&title={query}&types=anime-serial,anime&limit=100&with_material_data=true")]
    Task<KodikResults> SearchAnime(string query, string token);
    
    // [Get("/search?token={token}&title={query}&types=anime-serial,anime&limit=100&with_material_data=true&anime_genres={genres}")]
    // Task<KodikResults> SearchAnime(string query, string genres, string token = Constants.KodikKey);
}