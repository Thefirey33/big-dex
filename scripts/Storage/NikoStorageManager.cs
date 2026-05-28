using System.IO;
using bigdex.scripts.API.Types;
using Godot;

namespace bigdex.scripts.Storage;

public static class NikoStorageManager
{
    
    
    
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
}