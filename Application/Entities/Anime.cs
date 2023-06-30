using Application.AnimeSkipApi.Entities;
using Application.KodikApi.Entities;
using ShikimoriSharp.Classes;

namespace Application.Entities;

public class Anime
{
    public required AnimeID ShikimoriDetails { get; set; }
    public required KodikAnime KodikDetails { get; set; }
    public required Show? Timestamps { get; set; }
}