using ShikimoriSharp;
using ShikimoriSharp.Information;

namespace Anizavr.Backend.Application.ShikimoriApi;

public class ShikimoriClientWrapper : IShikimoriClient
{
    public ShikimoriClientWrapper(ShikimoriClient shikimoriClient)
    {
        Animes = new Animes(shikimoriClient.Client);
    }
    
    public Animes Animes { get; }
}