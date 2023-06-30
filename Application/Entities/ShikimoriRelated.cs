using Newtonsoft.Json;

namespace Application.Entities;

public class ShikimoriRelated
{
    [JsonProperty("relation")]
    public required string Relation { get; set; }

    [JsonProperty("relation_russian")]
    public required string RelationRussian { get; set; }

    [JsonProperty("anime")]
    public required ShikimoriAnime Anime { get; set; }
}