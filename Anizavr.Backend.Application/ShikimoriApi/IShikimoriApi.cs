using Refit;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace Anizavr.Backend.Application.ShikimoriApi;

public interface IShikimoriApi
{
    [Get("/animes?order={order}&status={status}&limit={limit}&page={page}&kind={kind}&season={season}")]
    Task<Anime[]> GetAnime(Order order, string status, int limit, int page, string kind, string season);
}