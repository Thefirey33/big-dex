#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using bigdex.scripts.API.Types;
using Godot;
using HttpClient = System.Net.Http.HttpClient;

namespace bigdex.scripts.API;

public static class NikoDexApi
{
    /// <summary>
    /// NikoDex API state enum.
    /// </summary>
    public enum ApiConnectionState
    {
        Connecting,
        Healthy,
        Unhealthy
    }

    /// <summary>
    /// Origin of the NikoDex API.
    /// </summary>
    public const string NikoDexOrigin = "https://nikodex.net/api";

    /// <summary>
    /// The HTTP Client in charge of making requests to the Dex.
    /// </summary>
    private static readonly HttpClient Client = new()
    {
        Timeout = new TimeSpan(0, 0, 5)
    };
    
    /// <summary>
    /// The Current NikoDex API Connection State.
    /// </summary>
    private static ApiConnectionState _nikoDexApiConnectionState = ApiConnectionState.Connecting;

    /// <summary>
    /// The Nikos that have been downloaded by the request handler.
    /// </summary>
    private static readonly List<Niko> Nikos = [];
    
    /// <summary>
    /// The amount of Nikos in the NikoDex currently.
    /// </summary>
    private static int _nikoCount;
    
    /// <summary>
    /// Set the Niko count omitted by the API.
    /// </summary>
    /// <param name="count"></param>
    public static void SetNikoCount(int count) =>  _nikoCount = count;
    
    /// <summary>
    /// Get the Niko count omitted by the API.
    /// </summary>
    /// <returns></returns>
    public static int GetNikoCount() => _nikoCount;

    /// <summary>
    /// Gets the current Niko count in the local DB.
    /// </summary>
    /// <returns>Niko Count.</returns>
    public static int GetNikoLocalDb() => Nikos.Count;
    
    /// <summary>
    /// Get the state of the NikoDex API.
    /// </summary>
    /// <returns>The state of the API.</returns>
    public static ApiConnectionState GetConnectionState() => _nikoDexApiConnectionState;
    
    /// <summary>
    /// Set the connection state of the NikoDex API.
    /// </summary>
    /// <param name="state">The state to set to.</param>
    public static void SetConnectionState(ApiConnectionState state) => _nikoDexApiConnectionState = state;

    #region API Request Handler
    
    private static async Task<HttpResponseMessage?> CreateHttpResponseMessage(string url)
    {
        GD.Print($"Attempting to dial: {url}...");
        var httpResponseMessage = await Client.GetAsync(url);
        GD.Print($"Recieved HTTP Response with code: {httpResponseMessage.StatusCode}");
        return !httpResponseMessage.IsSuccessStatusCode ? null : httpResponseMessage;
    }
    
    /// <summary>
    /// Request content from the API and convert it to a STRING.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>String Data.</returns>
    public static async Task<string?> RequestFromApiString(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        return stream != null ? await stream.Content.ReadAsStringAsync() : null;
    }
    
    /// <summary>
    /// Request content from the API and convert it to a STRUCT.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>Struct Data.</returns>
    public static async Task<T?> RequestFromApiStruct<T>(string url) where T: struct
    {
        var stream = await CreateHttpResponseMessage(url);
        if (stream == null)
            return null;
        var returnedResponse = await stream.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(returnedResponse);
    }
    
    /// <summary>
    /// Request content from the API and convert it to JSON DATA.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>Variant of the parsed data.</returns>
    public static async Task<Variant?> RequestFromApiJson(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        return stream != null ? Json.ParseString(await stream.Content.ReadAsStringAsync()) : (Variant?)null;
    }
    
    /// <summary>
    /// Request content from the API and convert it to BYTE DATA.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>Byte Buffer.</returns>
    public static async Task<byte[]?> RequestFromApiBytes(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        return stream != null ? await stream.Content.ReadAsByteArrayAsync() : null;
    }

    /// <summary>
    /// Create the reference text that the NikoDex API retrieved data uses to save itself.
    /// </summary>
    /// <param name="id">The ID number of the downloaded data to save to.</param>
    /// <returns>The created string that houses the filename without the EXTENSION.</returns>
    public static string CreateNikoFileText(int id) => $"niko-{id}";

    /// <summary>
    /// Download Niko data from the NikoDex API.
    /// This is a wrapper function.
    /// </summary>
    /// <param name="id">The ID of the Niko to reference.</param>
    /// <returns>Niko</returns>
    public static async Task<Niko?> DownloadNikoData(int id)
    {
        GD.Print($"Downloading Niko with ID {id}... Downloading Data...");
        var refNoik = await RequestFromApiStruct<Niko>($"{NikoDexOrigin}/data/niko?id={id}");

        if (!refNoik.HasValue)
        {
            GD.PushWarning("This Noik has no data, skipping...");
            return null;
        }
        
        var downloadedNoik = refNoik.Value;
        downloadedNoik.OriginalJsonInformation = JsonSerializer.Serialize(downloadedNoik);

        GD.Print($"Downloading Niko with ID {id}... Downloading Image...");
        var imgTexture = await ImageCreator.CreateImageTextureFromUrl($"{NikoDexOrigin}/image?id={id}");
        
        if (!imgTexture.IsLoaded)
        {
            GD.PushWarning("Warning, Image download failure! Skipping this Niko...");
            return null;
        }

        downloadedNoik.TextureData = imgTexture.Texture;
        return downloadedNoik;

    }

    /// <summary>
    /// Adds a Niko to the local DB.
    /// </summary>
    /// <param name="niko">Niko to add.</param>
    public static void AddNoikToList(Niko niko) => Nikos.Add(niko);

    /// <summary>
    /// Clear Nikos.
    /// </summary>
    public static void ClearNikoList() => Nikos.Clear();

    /// <summary>
    /// Checks if a specified Niko with an ID exists in the list.
    /// </summary>
    /// <param name="id">The ID of the Niko.</param>
    /// <returns>Specified Niko.</returns>
    public static bool DoesNikoExistInList(int id) => Nikos.Exists(niko => niko.Id == id);

    public static Niko GetNikoById(int id) => Nikos.Find(niko => niko.Id == id);

    /// <summary>
    /// Changes the specified Niko in the list.
    /// </summary>
    /// <param name="niko">Niko to change.</param>
    public static void ChangeNiko(Niko niko)
    {
        var index = Nikos.FindIndex(0, nikoSearch => niko.Id == nikoSearch.Id);
        Nikos[index] = niko;
    }

    #endregion
}