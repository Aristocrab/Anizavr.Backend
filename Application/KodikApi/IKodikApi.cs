using Application.KodikApi.Entities;
using Application.Shared;
using Refit;

namespace Application.KodikApi;

public interface IKodikApi
{
    [Get($"/search?token={Constants.KodikKey}&shikimori_id={{shikimoriId}}")]
    Task<KodikResults> GetAnime(long shikimoriId);
    
    [Get($"/search?token={Constants.KodikKey}&title={{query}}&types=anime-serial,anime&limit=100&with_material_data=true")]
    Task<KodikResults> SearchAnime(string query);
    
    [Get($"/search?token={Constants.KodikKey}&title={{query}}&types=anime-serial,anime&limit=100&with_material_data=true&anime_genres={{genres}}")]
    Task<KodikResults> SearchAnime(string query, string genres);
}