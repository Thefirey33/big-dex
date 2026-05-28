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
    /// The HTTP Client in charge of making requests to the Dex.
    /// </summary>
    private static readonly HttpClient Client = new HttpClient();
    /// <summary>
    /// The Current NikoDex API Connection State.
    /// </summary>
    private static ApiConnectionState _nikoDexApiConnectionState = ApiConnectionState.Connecting;

    /// <summary>
    /// The Nikos that have been downloaded by the request handler.
    /// </summary>
    private static List<Niko> _nikos = [];
    
    /// <summary>
    /// The amount of Nikos in the NikoDex currently.
    /// </summary>
    private static int _nikoCount = 0;
    
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
    
    private static async Task<HttpResponseMessage> CreateHttpResponseMessage(string url)
    {
        var httpResponseMessage = await Client.GetAsync(url);
        return !httpResponseMessage.IsSuccessStatusCode ? throw new HttpRequestException(httpResponseMessage.ReasonPhrase) : httpResponseMessage;
    }
    
    /// <summary>
    /// Request content from the API and convert it to a STRING.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>String Data.</returns>
    public static async Task<string> RequestFromApiString(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        return await stream.Content.ReadAsStringAsync();
    }
    
    /// <summary>
    /// Request content from the API and convert it to a STRUCT.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>Struct Data.</returns>
    public static async Task<(T, string)> RequestFromApiStruct<T>(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        var returnedResponse = await stream.Content.ReadAsStringAsync();
        return (JsonSerializer.Deserialize<T>(returnedResponse), returnedResponse);
    }
    
    /// <summary>
    /// Request content from the API and convert it to JSON DATA.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>Variant of the parsed data.</returns>
    public static async Task<Variant> RequestFromApiJson(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        return Json.ParseString(await stream.Content.ReadAsStringAsync());
    }
    
    /// <summary>
    /// Request content from the API and convert it to BYTE DATA.
    /// </summary>
    /// <param name="url">The URL of the NikoDex to use.</param>
    /// <returns>Byte Buffer.</returns>
    public static async Task<byte[]> RequestFromApiBytes(string url)
    {
        var stream = await CreateHttpResponseMessage(url);
        return await stream.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Create the reference text that the NikoDex API retrieved data uses to save itself.
    /// </summary>
    /// <param name="id">The ID number of the downloaded data to save to.</param>
    /// <returns>The created string that houses the filename without the EXTENSION.</returns>
    public static string CreateNikoFileText(int id) => $"niko-{id}";

    public static async Task<Niko?> DownloadNikoData(int id)
    {
        GD.Print($"Downloading Niko with ID {id}... Downloading Data...");
        var downloadedNoik = await RequestFromApiStruct<Niko>($"https://nikodex.net/api/data/niko?id={id}");
        downloadedNoik.Item1.OriginalJsonInformation = downloadedNoik.Item2;
        
        GD.Print($"Downloading Niko with ID {id}... Downloading Image...");
        var imgTexture = await ImageCreator.CreateImageTextureFromUrl($"https://nikodex.net/api/image?id={id}");
        if (!imgTexture.IsLoaded)
        {
            GD.PushWarning("Warning, Image download failure! Skipping this Niko...");
        }

        downloadedNoik.Item1.TextureData = imgTexture.Texture;
        return downloadedNoik.Item1;
    }

    public static void AddNoikToList(Niko niko)
    {
        _nikos.Add(niko);
    }

    #endregion
}