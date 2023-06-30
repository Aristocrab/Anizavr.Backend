using Newtonsoft.Json;

namespace Application.KodikApi.Entities;

public class Result
{
    public required string Shikimori_Id { get; set; }
    public required string Type { get; set; }
    public required string Link { get; set; }
    public required string Title { get; set; }
    public required string Title_Orig { get; set; }
    public required string Other_Title { get; set; }
    public required Translation Translation { get; set; }
    public required int Year { get; set; }
    public required int Episodes_Count { get; set; }
    public required List<string> Screenshots { get; set; }
    
    [JsonProperty("material_data")]
    public required MaterialData Material_Data { get; set; }
 }

public class MaterialData
{
    [JsonProperty("title")]
    public required string Title { get; set; }
    
    [JsonProperty("year")]
    public required int Year { get; set; }

    [JsonProperty("tagline")]
    public required string Tagline { get; set; }

    [JsonProperty("description")]
    public required string Description { get; set; }

    [JsonProperty("poster_url")]
    public required string Poster_Url { get; set; }
}