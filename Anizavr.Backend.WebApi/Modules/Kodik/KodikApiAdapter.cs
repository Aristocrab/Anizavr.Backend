using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.Domain.Entities.Kodik;

namespace Anizavr.Backend.WebApi.Modules.Kodik;

public class KodikApiAdapter : IKodikService
{
    private readonly IKodikApi _kodikApi;
    private readonly string _kodikKey;

    public KodikApiAdapter(IKodikApi kodikApi, string kodikKey)
    {
        _kodikApi = kodikApi;
        _kodikKey = kodikKey;
    }

    public Task<KodikResults> GetAnime(long shikimoriId)
    {
        return _kodikApi.GetAnime(shikimoriId,_kodikKey);
    }

    public Task<KodikResults> SearchAnime(string query)
    {
        return _kodikApi.SearchAnime(query, _kodikKey);
    }
}