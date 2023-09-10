using Anizavr.Backend.Application.KodikApi.Entities;

namespace Anizavr.Backend.Application.KodikApi;

public interface IKodikClient
{
    Task<KodikResults> GetAnime(long shikimoriId);
    Task<KodikResults> SearchAnime(string query);
}