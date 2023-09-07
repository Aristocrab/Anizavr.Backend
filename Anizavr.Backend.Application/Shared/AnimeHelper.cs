using Anizavr.Backend.Domain.Entities.Shikimori;
using ShikimoriSharp.Bases;

namespace Anizavr.Backend.Application.Shared;

public static class AnimeHelper
{
    public const long DeathNoteId = 1535;

    public static void FixDeathNotePoster(SmallRepresentation? anime)
    {
        if (anime is null) return;
        anime.Image.Original = $"/system/animes/original/{DeathNoteId}.jpg?1674340297";
    }
    
    public static List<AnimePreview> AnimeSimilarToDeathNote = new List<AnimePreview>
    {
        new()
        {
            Name = "Monstr",
            Episodes = 74,
            EpisodesAired = 74,
            Kind = "tv",
            Score = "8.86",
            Status = "released",
            Id = 19,
            Russian = "Монстр",
            Image = new Image
            {
                Original = "/system/animes/original/19.jpg?1674403441",
                Preview = "/system/animes/preview/19.jpg?1674403441",
                X96 = "/system/animes/x96/19.jpg?1674403441",
                X48 = "/system/animes/x48/19.jpg?1674403441"
            },
            Url = "/animes/19-monster"
        },
        new()
        {
            Name = "Эхо террора",
            Episodes = 11,
            EpisodesAired = 11,
            Kind = "tv",
            Score = "8.1",
            Status = "released",
            Id = 23283,
            Russian = "Эхо террора",
            Image = new Image
            {
                Original = "/system/animes/original/23283.jpg?1675112518",
                Preview = "/system/animes/preview/23283.jpg?1675112518",
                X96 = "/system/animes/x96/23283.jpg?1675112518",
                X48 = "/system/animes/x48/23283.jpg?1675112518"
            },
            Url = "/animes/23283-zankyou-no-terror"
        },
        new()
        {
            Name = "Psycho-Pass",
            Episodes = 22,
            EpisodesAired = 22,
            Kind = "tv",
            Score = "8.34",
            Status = "released",
            Id = 13601,
            Russian = "Психопаспорт",
            Image = new Image
            {
                Original = "/system/animes/original/13601.jpg?1675110089",
                Preview = "/system/animes/preview/13601.jpg?1675110089",
                X96 = "/system/animes/x96/13601.jpg?1675110089",
                X48 = "/system/animes/x48/13601.jpg?1675110089"
            },
            Url = "/animes/z13601-psycho-pass"
        },
        new()
        {
            Name = "Дневник будущего",
            Episodes = 26,
            EpisodesAired = 26,
            Kind = "tv",
            Score = "7.42",
            Status = "released",
            Id = 10620,
            Russian = "Дневник будущего",
            Image = new Image
            {
                Original = "/system/animes/original/10620.jpg?1674357768",
                Preview = "/system/animes/preview/10620.jpg?1674357768",
                X96 = "/system/animes/x96/10620.jpg?1674357768",
                X48 = "/system/animes/x48/10620.jpg?1674357768"
            },
            Url = "/animes/10620-mirai-nikki-tv"
        },
        new()
        {
            Name = "Врата Штейна",
            Episodes = 24,
            EpisodesAired = 24,
            Kind = "tv",
            Score = "9.08",
            Status = "released",
            Id = 9253,
            Russian = "Врата Штейна",
            Image = new Image
            {
                Original = "/system/animes/original/9253.jpg?1674355853",
                Preview = "/system/animes/preview/9253.jpg?1674355853",
                X96 = "/system/animes/x96/9253.jpg?1674355853",
                X48 = "/system/animes/x48/9253.jpg?1674355853"
            },
            Url = "/animes/z9253-steins-gate"
        },
        new()
        {
            Name = "The Promised Neverland",
            Episodes = 12,
            EpisodesAired = 12,
            Kind = "tv",
            Score = "8.51",
            Status = "released",
            Id = 37779,
            Russian = "Обещанный Неверленд",
            Image = new Image
            {
                Original = "/system/animes/original/37779.jpg?1675118347",
                Preview = "/system/animes/preview/37779.jpg?1675118347",
                X96 = "/system/animes/x96/37779.jpg?1675118347",
                X48 = "/system/animes/x48/37779.jpg?1675118347"
            },
            Url = "/animes/z37779-yakusoku-no-neverland"
        },
        new()
        {
            Name = "Higurashi: When They Cry",
            Episodes = 26,
            EpisodesAired = 26,
            Kind = "tv",
            Score = "7.89",
            Status = "released",
            Id = 934,
            Russian = "Когда плачут цикады",
            Image = new Image
            {
                Original = "/system/animes/original/934.jpg?1674337958",
                Preview = "/system/animes/preview/934.jpg?1674337958",
                X96 = "/system/animes/x96/934.jpg?1674337958",
                X48 = "/system/animes/x48/934.jpg?1674337958"
            },
            Url = "/animes/z934-higurashi-no-naku-koro-ni"
        },
        new()
        {
            Name = "Darker than Black: Kuro no Keiyakusha",
            Episodes = 25,
            EpisodesAired = 25,
            Kind = "tv",
            Score = "8.06",
            Status = "released",
            Id = 2025,
            Russian = "Темнее чёрного",
            Image = new Image
            {
                Original = "/system/animes/original/2025.jpg?1674342128",
                Preview = "/system/animes/preview/2025.jpg?1674342128",
                X96 = "/system/animes/x96/2025.jpg?1674342128",
                X48 = "/system/animes/x48/2025.jpg?1674342128"
            },
            Url = "/animes/z2025-darker-than-black-kuro-no-keiyakusha"
        },
        new()
        {
            Name = "Code Geass: Hangyaku no Lelouch",
            Episodes = 25,
            EpisodesAired = 0,
            Kind = "tv",
            Score = "8.7",
            Status = "released",
            Id = 1575,
            Russian = "Код Гиас: Восставший Лелуш",
            Image = new Image
            {
                Original = "/system/animes/original/1575.jpg?1674340438",
                Preview = "/system/animes/preview/1575.jpg?1674340438",
                X96 = "/system/animes/x96/1575.jpg?1674340438",
                X48 = "/system/animes/x48/1575.jpg?1674340438"
            },
            Url = "/animes/1575-code-geass-hangyaku-no-lelouch"
        },
        new()
        {
            Name = "Mousou Dairinin",
            Episodes = 13,
            EpisodesAired = 13,
            Kind = "tv",
            Score = "7.67",
            Status = "released",
            Id = 323,
            Russian = "Агент паранойи",
            Image = new Image
            {
                Original = "/system/animes/original/323.jpg?1674334171",
                Preview = "/system/animes/preview/323.jpg?1674334171",
                X96 = "/system/animes/x96/323.jpg?1674334171",
                X48 = "/system/animes/x48/323.jpg?1674334171"
            },
            Url = "/animes/323-mousou-dairinin"
        }
  
    };
}