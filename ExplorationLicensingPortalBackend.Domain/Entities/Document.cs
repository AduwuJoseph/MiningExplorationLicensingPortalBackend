using ExplorationLicensingPortalBackend.Domain.Enums;

namespace ExplorationLicensingPortalBackend.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; private set; }
        public Guid ApplicationId { get; private set; }
        public DocumentType DocumentType { get; private set; }
        public string FileName { get; private set; } = null!;
        public string BlobUrl { get; private set; } = null!;
        public DateTime UploadedAt { get; private set; }

        private Document() { }

        public static Document Create(Guid applicationId, DocumentType documentType, string fileName, string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl)) throw new ArgumentException("Blob URL is required");
            return new Document
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                DocumentType = documentType,
                FileName = fileName,
                BlobUrl = blobUrl,
                UploadedAt = DateTime.UtcNow
            };
        }
    }
}
