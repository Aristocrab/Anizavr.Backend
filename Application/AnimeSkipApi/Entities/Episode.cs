namespace Application.AnimeSkipApi.Entities;

public class Episode
{
    public required string Name { get; set; }
    public required List<Timestamp> Timestamps { get; set; }
}