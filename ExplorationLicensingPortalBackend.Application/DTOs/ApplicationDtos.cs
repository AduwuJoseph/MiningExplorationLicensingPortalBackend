using ExplorationLicensingPortalBackend.Domain.Enums;

namespace ExplorationLicensingPortalBackend.Application.DTOs
{
    public record CreateApplicationRequest(
        string CompanyName,
        string Email,
        string Address,
        string ContactPersonName,
        string PhoneNumber,
        string State,
        string LocalGovernmentArea,
        ApplicationType ApplicationType,
        string MineralTypes,
        string? ExportCountry = null,
        int? CadastreUnits = null
    );

    public record CreateApplicationResponse(Guid ApplicationId, ApplicationStatus Status, decimal Fee);

    public record UploadDocumentRequest(Guid ApplicationId, DocumentType DocumentType);

    public record UploadDocumentResponse(Guid DocumentId, string BlobUrl, DocumentType DocumentType);

    public record GenerateRRRRequest(Guid ApplicationId);

    public record GenerateRRRResponse(string RRR, decimal Amount, Guid ApplicationId);

    public record SubmitApplicationRequest(Guid ApplicationId);

    public record SubmitApplicationResponse(Guid ApplicationId, ApplicationStatus Status, DateTime SubmittedAt);
}
