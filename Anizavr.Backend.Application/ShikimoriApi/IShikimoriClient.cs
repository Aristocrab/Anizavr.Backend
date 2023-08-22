using ShikimoriSharp.Information;

namespace Anizavr.Backend.Application.ShikimoriApi;

public interface IShikimoriClient
{
    Animes Animes { get; }
}