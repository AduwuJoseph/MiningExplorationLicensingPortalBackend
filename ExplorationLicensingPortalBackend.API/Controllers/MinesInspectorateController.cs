using ExplorationLicensingPortalBackend.Application.DTOs;
using ExplorationLicensingPortalBackend.Application.Interfaces;
using ExplorationLicensingPortalBackend.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExplorationLicensingPortalBackend.API.Controllers
{
    /// <summary>
    /// Mines Inspectorate Department — Licence and Permit Applications
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MinesInspectorateController(IApplicationService service) : ControllerBase
    {
        /// <summary>Create a new licence/permit application</summary>
        /// <remarks>
        /// Initiates one of the following application types:
        ///
        /// | ApplicationType | Regulation | Fee |
        /// |---|---|---|
        /// | LicenceToPurchaseAndPossess | Reg 133 | ₦100,000 per mineral |
        /// | PermitToExportCommercial | Reg 131 | ₦100,000 flat |
        /// | PermitToExportSamples | Reg 132 | ₦10,000 flat |
        ///
        /// **Sample request (Reg 133):**
        /// ```json
        /// {
        ///   "companyName": "Acme Mining Ltd",
        ///   "email": "info@acmemining.com",
        ///   "address": "12 Industrial Layout, Abuja",
        ///   "contactPersonName": "John Doe",
        ///   "phoneNumber": "08012345678",
        ///   "state": "FCT",
        ///   "localGovernmentArea": "Abuja Municipal",
        ///   "applicationType": "LicenceToPurchaseAndPossess",
        ///   "mineralTypes": "Gold,Iron Ore"
        /// }
        /// ```
        /// </remarks>
        /// <response code="201">Application created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreateApplicationResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request)
        {
            var result = await service.CreateAsync(request);
            return CreatedAtAction(nameof(Create), ApiResponse<CreateApplicationResponse>.Ok(result));
        }

        /// <summary>Upload a required document for an application</summary>
        /// <remarks>
        /// Upload one document at a time. Call this endpoint once per required document.
        ///
        /// **Required documents by type:**
        ///
        /// *Reg 133 — Licence to Purchase and Possess:*
        /// CertificateOfIncorporation, CAC2, CAC7, TaxClearanceCertificate, AttestationOfNonConviction,
        /// BankersGuarantee, SourceOfSupplyLetter, MiningEngineerCredentials,
        /// TechnicalPersonEmploymentLetter, TechnicalPersonAcceptanceLetter, COMEGSeal
        ///
        /// *Reg 131 — Permit to Export (Commercial):*
        /// CertificateOfIncorporation, CAC2, CAC7, TaxClearanceCertificate, NEPCRegistration,
        /// SourceOfSupplyLetter, RoyaltyPaymentEvidence, ExportContractOrReason, ZonalInspectionReport
        ///
        /// *Reg 132 — Permit to Export Samples:*
        /// CertificateOfIncorporation, CAC2, CAC7, LetterOfIntroduction,
        /// ForeignLabCorrespondence, TSAPaymentEvidence, SourceOfSupplyLetter, ZonalInspectionReport
        /// </remarks>
        /// <param name="applicationId">The application ID returned from the create endpoint</param>
        /// <param name="request">Document type metadata</param>
        /// <param name="file">The document file to upload</param>
        /// <response code="200">Document uploaded successfully</response>
        /// <response code="400">File missing or invalid</response>
        /// <response code="404">Application not found</response>
        [HttpPost("{applicationId}/documents")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<UploadDocumentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadDocument(
            Guid applicationId,
            [FromForm] UploadDocumentRequest request,
            IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("File is required"));

            using var stream = file.OpenReadStream();
            var result = await service.UploadDocumentAsync(
                request with { ApplicationId = applicationId }, stream, file.FileName);

            return Ok(ApiResponse<UploadDocumentResponse>.Ok(result));
        }

        /// <summary>Generate a Remita RRR (Retrieval Reference Number) for payment</summary>
        /// <remarks>
        /// Computes the application fee and generates a Remita RRR.
        /// The applicant uses the RRR to pay at any Remita-enabled bank or online.
        ///
        /// **Fee schedule:**
        /// - LicenceToPurchaseAndPossess: ₦100,000 × number of minerals
        /// - PermitToExportCommercial: ₦100,000
        /// - PermitToExportSamples: ₦10,000
        /// </remarks>
        /// <param name="applicationId">The application ID</param>
        /// <response code="200">RRR generated successfully</response>
        /// <response code="404">Application not found</response>
        [HttpPost("{applicationId}/generate-rrr")]
        [ProducesResponseType(typeof(ApiResponse<GenerateRRRResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateRRR(Guid applicationId)
        {
            var result = await service.GenerateRRRAsync(new GenerateRRRRequest(applicationId));
            return Ok(ApiResponse<GenerateRRRResponse>.Ok(result));
        }

        /// <summary>Submit the application for processing</summary>
        /// <remarks>
        /// Finalises and submits the application to the Mines Inspectorate Department.
        /// All required documents must be uploaded before submission.
        /// </remarks>
        /// <param name="applicationId">The application ID</param>
        /// <response code="200">Application submitted successfully</response>
        /// <response code="400">Documents not yet uploaded</response>
        /// <response code="404">Application not found</response>
        [HttpPost("{applicationId}/submit")]
        [ProducesResponseType(typeof(ApiResponse<SubmitApplicationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Submit(Guid applicationId)
        {
            var result = await service.SubmitAsync(new SubmitApplicationRequest(applicationId));
            return Ok(ApiResponse<SubmitApplicationResponse>.Ok(result));
        }
    }
}
