using Anizavr.Backend.Application.KodikApi.Entities;
using ShikimoriSharp.Classes;

namespace Anizavr.Backend.Application.Entities;

public class Anime
{
    public required AnimeID ShikimoriDetails { get; set; }
    public required KodikResults KodikDetails { get; set; }
    public List<object>? Timestamps { get; set; } = null;
}