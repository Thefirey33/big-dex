using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using bigdex.scripts.API;
using bigdex.scripts.API.Types;
using Godot;

namespace bigdex.scripts.Storage;

/// <summary>
/// The general storage system of the game.
/// It stores the Nikos and saves them.
/// </summary>
public static class NikoStorageManager
{
    
    /// <summary>
    /// The individual states of the game's loaders.
    /// </summary>
    public enum GameLoaderState
    {
        Uncompressing,
        Preloading,
        DownloadingAndSaving,
        Compressing,
        Done
    }

    /// <summary>
    /// The warning that will be shown in the compressed zip file to not modify.
    /// </summary>
    private const string ZipWarning = "Do not modify this file. This file contains data for the game's assets.";
    
    /// <summary>
    /// The loading state of the game.
    /// </summary>
    private static GameLoaderState _loaderState = GameLoaderState.Uncompressing;

    /// <summary>
    /// Get the state of the loader state.
    /// </summary>
    /// <returns>The state of the game asset loader.</returns>
    public static GameLoaderState GetLoaderState() => _loaderState;
    
    /// <summary>
    /// Sets the state of the loader state.
    /// </summary>
    /// <param name="loaderState">State to set to.</param>
    public static void SetLoaderState(GameLoaderState loaderState) =>  _loaderState = loaderState;
    
    /// <summary>
    /// Where all the files of the game are stored.
    /// </summary>
    private static string GameFileStoragePath => ProjectSettings.GlobalizePath("user://");

    /// <summary>
    /// The data where all the Noiks are stored.
    /// </summary>
    private const string GameNikoStorageFile = "data.tdat";
    
    /// <summary>
    /// Path to the zip file.
    /// </summary>
    public static string ZipFilePath => Path.Join(GameFileStoragePath, GameNikoStorageFile);
    
    /// <summary>
    /// Store a Niko in the user's game data directory.
    /// </summary>
    /// <param name="niko">The Noik to store.</param>
    public static void StoreNiko(Niko niko)
    {
        var nikoId = niko.Id;
        
        var nikoPath = ProjectSettings.GlobalizePath($"user://noik-{nikoId}.json");
        File.WriteAllText(nikoPath, niko.OriginalJsonInformation);

        var imgData = niko.TextureData.GetImage();
        imgData.SavePng($"user://noik-{nikoId}.png");
    }
    
    /// <summary>
    /// Import all the Nikos already pre-cached.
    /// </summary>
    public static void ImportBeforeAssets()
    {
        foreach (var directoryEntry in DirAccess.GetFilesAt("user://"))
        {
            var pathExtension = Path.GetExtension(directoryEntry);
            var globalPath = ProjectSettings.GlobalizePath($"user://{directoryEntry}");
			
            switch (pathExtension)
            {
                case ".json":
                    var jsonText = File.ReadAllText(globalPath);
					
                    // Check if the file is epty, if it is, skip the file.
                    // So the JSON Serializer doesn't throw an error.
                    if (jsonText.Length <= 0)
                    {
                        GD.PushWarning("Skipping JSON importing, file is empty. Deleting.");
                        File.Delete(globalPath);
                        return;
                    }
					
                    var deserializedNiko = JsonSerializer.Deserialize<Niko>(jsonText);
					
                    NikoDexApi.AddNoikToList(deserializedNiko);
                    break;
				
                case ".png":
                    // Load an already existing PNG to the game.
                    
                    var imgId = Path.GetFileNameWithoutExtension(directoryEntry[(directoryEntry.LastIndexOf('-') + 1)..]);

                    if (int.TryParse(imgId, out var id))
                    {
                        var niko = NikoDexApi.GetNikoById(id);

                        var img = ImageCreator.CreateImageTextureFromByteBuffer(File.ReadAllBytes(globalPath));
                        niko.TextureData = img;
						
                        GD.Print($"Imported image data for Niko ID: {niko.Id}, Rid: {niko.TextureData.GetRid()}");
                        NikoDexApi.ChangeNiko(niko);
                    }
                    break;
				
                default:
                    GD.PushWarning($"Unsupported type: {pathExtension} detected.");
                    continue;
            }
        }
    }

    public static void CompressAllAssets()
    {
        using var zipArchive = ZipFile.Open(ZipFilePath, ZipArchiveMode.Create);
        zipArchive.Comment = ZipWarning;
        
        foreach (var fileEntry in Directory.EnumerateFiles(GameFileStoragePath, "*.*").Where(s => s.EndsWith(".json") || s.EndsWith(".png")))
        {
            zipArchive.CreateEntryFromFile(fileEntry, Path.GetFileName(fileEntry));
            
            // Delete the file after the compression is complete.
            File.Delete(fileEntry);
        }
        
        SetLoaderState(GameLoaderState.Done);
    }

    public static void UncompressAllAssets()
    {
        ZipFile.ExtractToDirectory(ZipFilePath, GameFileStoragePath);
        File.Delete(ZipFilePath);
    }
}