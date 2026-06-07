using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using bigdex.scripts.API;
using bigdex.scripts.API.Types;
using bigdex.scripts.Storage;
using Godot;

namespace bigdex.scripts;

public partial class ApiPreloader : Node2D
{
	/// <summary>
	/// Global randomizer.
	/// </summary>
	private readonly Random _random = new();
	
	/// <summary>
	/// The amount from the total COUNT of items that can be downloaded.
	/// </summary>
	private const int MaxDownloadPercentage = 25;

	/// <summary>
	/// Maximum amount of stuff the Application can download.
	/// </summary>
	private int _downloadTotalCount;
	

	public override void _EnterTree()
	{
		NikoStorageManager.SetLoaderState(NikoStorageManager.GameLoaderState.Uncompressing);
		
		// First, if the assets already exist, uncompress them.
		// Compression is done by the game to eliminate cluttery files.
		if (File.Exists(NikoStorageManager.ZipFilePath))
			NikoStorageManager.UncompressAllAssets();
		
		NikoStorageManager.SetLoaderState(NikoStorageManager.GameLoaderState.Preloading);
		// Import all the current existing assets.
		NikoStorageManager.ImportBeforeAssets();
		
		NikoStorageManager.SetLoaderState(NikoStorageManager.GameLoaderState.DownloadingAndSaving);
		Task.Run(async () =>
		{
			NikoDexApi.SetConnectionState(NikoDexApi.ApiConnectionState.Connecting);
			// Check if the NikoDex API is healthy.
			// Check if the API's not dead.
			var response = await NikoDexApi.RequestFromApiString($"{NikoDexApi.NikoDexOrigin}/ping");
			
			NikoDexApi.SetConnectionState(NikoDexApi.ApiConnectionState.Healthy);
			if (response == null)
			{
				GD.PushWarning("API is dead, switching to offline mode...");
				NikoDexApi.SetConnectionState(NikoDexApi.ApiConnectionState.Unhealthy);
			}
				
			// Set the amount of Noiks currently available to fetch through in the API.
			var apiCountResponse = await NikoDexApi.RequestFromApiString($"{NikoDexApi.NikoDexOrigin}/data/count");
			
			if (apiCountResponse == null)
			{
				GD.PushWarning("Could not fetch API count, skipping.");
				return;
			}

			var nikodexNoikCountRequest = int.Parse(apiCountResponse);
			NikoDexApi.SetNikoCount(nikodexNoikCountRequest);
			GD.Print($"Total amount of Nikos that can be downloaded: {nikodexNoikCountRequest}, PERCENTAGE: {MaxDownloadPercentage}");

			NikoDexApi.ClearNikoList();
			_downloadTotalCount = (NikoDexApi.GetNikoCount() - NikoDexApi.GetNikoLocalDb()) / MaxDownloadPercentage;
			
			
			for (var i = 0; i <= _downloadTotalCount; i++)
			{
				int nikoId;
				
				// Select a random Niko ID to select from the NikoDex DB.
				do
				{
					nikoId = (_random.Next() % NikoDexApi.GetNikoCount()) + 1;
				} while (NikoDexApi.DoesNikoExistInList(nikoId));
				
				// Download the specified Niko data and store in the local storage.
				var downloadedNikoData = await NikoDexApi.DownloadNikoData(nikoId);
				if (!downloadedNikoData.HasValue) continue;
				
				var nikoData = downloadedNikoData.Value;
				NikoDexApi.AddNoikToList(nikoData);
				NikoStorageManager.StoreNiko(nikoData);
			}
			
			NikoStorageManager.SetLoaderState(NikoStorageManager.GameLoaderState.Compressing);
			NikoStorageManager.CompressAllAssets();

			GetTree().ChangeSceneToFile("res://scenes/sc_main.tscn");
		});
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}
}