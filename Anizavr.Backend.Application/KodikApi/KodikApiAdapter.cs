using Anizavr.Backend.Application.KodikApi.Entities;

namespace Anizavr.Backend.Application.KodikApi;

public class KodikApiAdapter : IKodikClient
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