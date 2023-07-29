using Application.AnimeSkipApi.Entities;
using Application.KodikApi.Entities;
using ShikimoriSharp.Classes;

namespace Application.Entities;

public class Anime
{
    public required AnimeID ShikimoriDetails { get; set; }
    public required KodikResults KodikDetails { get; set; }
    public required List<EpisodeTimestamps>? Timestamps { get; set; }
}