using System.Text.Json.Serialization;

namespace bigdex.scripts.API.Types;

public struct Ability
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("niko_id")]
    public int NikoId { get; set; }
}