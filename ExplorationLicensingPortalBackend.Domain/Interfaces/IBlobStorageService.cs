namespace ExplorationLicensingPortalBackend.Domain.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string containerName);
    }
}
