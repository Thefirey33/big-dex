using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace bigdex.scripts.API.Types;

public struct Niko
{
    /// <summary>
    /// The stored texture data. This should NOT be given a value at first, later loaded by the IMAGE request!
    /// </summary>
    [JsonIgnore]
    public ImageTexture TextureData;
    
    
    /// <summary>
    /// The stored original JSON data.
    /// </summary>
    [JsonIgnore]
    public string OriginalJsonInformation;
    
    /// <summary>
    /// The ID of the Niko.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the Niko.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    /// <summary>
    /// The name of the author of this Niko.
    /// </summary>
    [JsonPropertyName("author_name")]
    public string AuthorName { get; set; }
    
    /// <summary>
    /// The full description of this Niko, requested from the API.
    /// </summary>
    [JsonPropertyName("full_desc")]
    public string FullDescription { get; set; }
    
    /// <summary>
    /// The short description of this Niko.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    /// <summary>
    /// The abilities of this Niko.
    /// </summary>
    [JsonPropertyName("abilities")]
    public List<Ability> Abilities { get; set; }
    
    /// <summary>
    /// If this Niko is blacklisted.
    /// Basically, no petpet allowed.
    /// </summary>
    [JsonPropertyName("is_blacklisted")]
    public bool IsBlacklisted { get; set; }
    
    /// <summary>
    /// The Author ID of this Niko.
    /// This might be null, as some Nikosonas do NOT contain an author!
    /// </summary>
    [JsonPropertyName("author_id")]
    public int? AuthorId { get; set; }
    
}