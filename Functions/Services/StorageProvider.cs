using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Frederikskaj2.EmailAgent.Functions
{
    internal class StorageProvider : IStorageProvider
    {
        private const int Conflict = 409;
        private const int NotFound = 404;
        private readonly BlobServiceClient client;
        private readonly StorageOptions options;

        public StorageProvider(IOptions<StorageOptions> options)
        {
            this.options = options.Value;

            client = new BlobServiceClient(this.options.ConnectionString);
        }

        public async Task<T?> Read<T>(string name) where T : class
        {
            var containerClient = await GetContainerClient();
            var blobClient = containerClient.GetBlobClient(name);
            try
            {
                var response = await blobClient.DownloadAsync();
                return response.Value.Content.Deserialize<T>();
            }
            catch (RequestFailedException exception) when (exception.Status == NotFound)
            {
                return null;
            }
        }

        public async Task Write<T>(string name, T value) where T : class
        {
            var containerClient = await GetContainerClient();
            var blobClient = containerClient.GetBlobClient(name);
            using var stream = new MemoryStream();
            stream.Serialize(value);
            stream.Seek(0, SeekOrigin.Begin);
            await blobClient.UploadAsync(stream, true);
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = "application/json" });
        }

        private async Task<BlobContainerClient> GetContainerClient()
        {
            try
            {
                return await client.CreateBlobContainerAsync(options.ContainerName);
            }
            catch (RequestFailedException exception) when (exception.Status == Conflict)
            {
                return client.GetBlobContainerClient(options.ContainerName);
            }
        }
    }
}
