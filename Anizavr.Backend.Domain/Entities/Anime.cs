using Anizavr.Backend.Domain.Entities.Kodik;
using ShikimoriSharp.Classes;

namespace Anizavr.Backend.Domain.Entities;

public class Anime
{
    public required AnimeID ShikimoriDetails { get; set; }
    public required KodikResults KodikDetails { get; set; }
    public List<object>? Timestamps { get; set; } = null;
}