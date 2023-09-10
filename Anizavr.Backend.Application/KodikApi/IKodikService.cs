using Anizavr.Backend.Domain.Entities.Kodik;

namespace Anizavr.Backend.Application.KodikApi;

public interface IKodikService
{
    Task<KodikResults> GetAnime(long shikimoriId);
    Task<KodikResults> SearchAnime(string query);
}