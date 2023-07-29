using Newtonsoft.Json;

namespace Anizavr.Backend.Application.ShikimoriApi;

public class ShikimoriRelated
{
    [JsonProperty("relation")]
    public required string Relation { get; set; }

    [JsonProperty("relation_russian")]
    public required string RelationRussian { get; set; }

    [JsonProperty("anime")]
    public required AnimePreview Anime { get; set; }
}