using Azure.Storage.Blobs;
using ExplorationLicensingPortalBackend.Domain.Interfaces;

namespace ExplorationLicensingPortalBackend.Infrastructure.Services
{
    public class BlobStorageService(BlobServiceClient blobServiceClient) : IBlobStorageService
    {
        public async Task<string> UploadAsync(Stream fileStream, string fileName, string containerName)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient(fileName);
            await blob.UploadAsync(fileStream, overwrite: true);
            return blob.Uri.ToString();
        }
    }
}
