using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SnapRoom.Common.Utils
{
    public class CoreHelper
	{
		public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);

		public static async Task<string> UploadImage(IFormFile imageFile)
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			string connectionString = config.GetConnectionString("BlobContainer")!;
			string containerName = "snaproom";
			var blobServiceClient = new BlobServiceClient(connectionString);
			var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

			// Optional: create container if it doesn't exist
			await containerClient.CreateIfNotExistsAsync();

			// Use original filename or generate a unique one
			string randomString = Guid.NewGuid().ToString().Substring(0, 8);
			string blobName = imageFile.FileName + "_" + randomString;
			// Get a BlobClient representing the blob to upload to
			var blobClient = containerClient.GetBlobClient(blobName);

			// Upload the image file stream to the blob with Content-Type set
			using (var stream = imageFile.OpenReadStream())
			{
				var options = new BlobUploadOptions
				{
					HttpHeaders = new BlobHttpHeaders { ContentType = imageFile.ContentType }
				};

				await blobClient.UploadAsync(stream, options);
			}


			return $"https://dataimage.blob.core.windows.net/snaproom/{blobName}";
		}

		public static async Task DeleteImage(string imageSource)
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			string connectionString = config.GetConnectionString("BlobContainer")!;
			string containerName = "snaproom";
			var blobServiceClient = new BlobServiceClient(connectionString);
			var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

			string prefix = $"https://dataimage.blob.core.windows.net/{containerName}/";
			if (!imageSource.StartsWith(prefix))
				throw new ArgumentException("Invalid image source URL");

			string blobName = imageSource.Substring(prefix.Length);

			// Get the BlobClient and delete the blob
			var blobClient = containerClient.GetBlobClient(blobName);
			await blobClient.DeleteIfExistsAsync();
		}

	}
}
