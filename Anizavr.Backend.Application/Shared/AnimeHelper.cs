using ShikimoriSharp.Bases;

namespace Anizavr.Backend.Application.Shared;

public class AnimeHelper
{
    public const long DeathNoteId = 1535;

    public static void FixDeathNotePoster(SmallRepresentation? anime)
    {
        if (anime is null) return;
        anime.Image.Original = $"/system/animes/original/{DeathNoteId}.jpg?1674340297";
    }
}