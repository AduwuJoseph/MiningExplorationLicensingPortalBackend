using ExplorationLicensingPortalBackend.Application.DTOs;

namespace ExplorationLicensingPortalBackend.Application.Interfaces
{
    public interface IApplicationService
    {
        Task<CreateApplicationResponse> CreateAsync(CreateApplicationRequest request);
        Task<UploadDocumentResponse> UploadDocumentAsync(UploadDocumentRequest request, Stream fileStream, string fileName);
        Task<GenerateRRRResponse> GenerateRRRAsync(GenerateRRRRequest request);
        Task<SubmitApplicationResponse> SubmitAsync(SubmitApplicationRequest request);
    }
}
