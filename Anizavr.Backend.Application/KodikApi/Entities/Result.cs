using Newtonsoft.Json;

namespace Anizavr.Backend.Application.KodikApi.Entities;

public class Result
{
    public string? Shikimori_Id { get; set; }
    public required string Type { get; set; }
    public required string Link { get; set; }
    public required string Title { get; set; }
    public string? Title_Orig { get; set; }
    public string? Other_Title { get; set; }
    public required Translation Translation { get; set; }
    public int Year { get; set; }
    public int? Episodes_Count { get; set; }
    public required List<string> Screenshots { get; set; }
    
    [JsonProperty("material_data")]
    public MaterialData? Material_Data { get; set; }
 }

public class MaterialData
{
    [JsonProperty("title")]
    public required string Title { get; set; }
    
    [JsonProperty("year")]
    public int? Year { get; set; }

    [JsonProperty("tagline")]
    public string? Tagline { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("poster_url")]
    public string? Poster_Url { get; set; }
    
    [JsonProperty("shikimori_rating")]
    public float? Shikimori_rating { get; set; }
}