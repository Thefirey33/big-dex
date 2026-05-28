using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using bigdex.scripts.API;
using bigdex.scripts.API.Types;
using bigdex.scripts.Storage;
using Godot;

namespace bigdex.scripts;

public partial class ApiPreloader : Node2D
{
	/// <summary>
	/// The amount from the total COUNT of items that can be downloaded.
	/// </summary>
	private const int MaxDownloadPercentage = 15;

	public override void _EnterTree()
	{
		Task.Run(async () =>
		{
			// Check if the NikoDex API is healthy.
			// Check if the API's not dead.
			var pingCheckResponse = await NikoDexApi.RequestFromApiString("https://nikodex.net/api/ping");
			NikoDexApi.SetConnectionState(NikoDexApi.ApiConnectionState.Healthy);
				
			// Set the amount of Noiks currently available to fetch through in the API.
			var nikodexNoikCountRequest = int.Parse(await NikoDexApi.RequestFromApiString("https://nikodex.net/api/data/count"));
			NikoDexApi.SetNikoCount(nikodexNoikCountRequest);
			GD.Print($"Total amount of Nikos that can be downloaded: {nikodexNoikCountRequest}");

			var randomNumberGenerator = new RandomNumberGenerator();
			for (var i = 0; i < NikoDexApi.GetNikoCount() / MaxDownloadPercentage; i++)
			{
				var noikId = randomNumberGenerator.RandiRange(1, NikoDexApi.GetNikoCount());
				var downloadedNikoData = await NikoDexApi.DownloadNikoData(noikId);

				if (!downloadedNikoData.HasValue) continue;
					
				var nikoData = downloadedNikoData.Value;
				NikoDexApi.AddNoikToList(nikoData);
				NikoStorageManager.StoreNiko(nikoData);
			}
		});
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}
}