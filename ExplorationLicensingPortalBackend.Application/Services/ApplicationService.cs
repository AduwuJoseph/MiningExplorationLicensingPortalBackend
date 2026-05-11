using ExplorationLicensingPortalBackend.Application.DTOs;
using ExplorationLicensingPortalBackend.Application.Interfaces;
using ExplorationLicensingPortalBackend.Domain.Entities;
using ExplorationLicensingPortalBackend.Domain.Interfaces;

namespace ExplorationLicensingPortalBackend.Application.Services
{
    public class ApplicationService(
        IApplicationRepository repository,
        IBlobStorageService blobStorage,
        IRemitaService remita) : IApplicationService
    {
        private const string BlobContainer = "mining-applications";

        public async Task<CreateApplicationResponse> CreateAsync(CreateApplicationRequest req)
        {
            var app = Domain.Entities.Application.Create(
                req.CompanyName, req.Email, req.Address,
                req.ContactPersonName, req.PhoneNumber,
                req.State, req.LocalGovernmentArea,
                req.ApplicationType, req.MineralTypes,
                req.ExportCountry, req.CadastreUnits);

            await repository.CreateAsync(app);
            return new CreateApplicationResponse(app.Id, app.Status, app.CalculateFee());
        }

        public async Task<UploadDocumentResponse> UploadDocumentAsync(
            UploadDocumentRequest req, Stream fileStream, string fileName)
        {
            var app = await GetOrThrowAsync(req.ApplicationId);

            var blobUrl = await blobStorage.UploadAsync(
                fileStream, $"{req.ApplicationId}/{fileName}", BlobContainer);

            var doc = Document.Create(app.Id, req.DocumentType, fileName, blobUrl);
            app.AddDocument(doc);
            await repository.UpdateAsync(app);

            return new UploadDocumentResponse(doc.Id, doc.BlobUrl, doc.DocumentType);
        }

        public async Task<GenerateRRRResponse> GenerateRRRAsync(GenerateRRRRequest req)
        {
            var app = await GetOrThrowAsync(req.ApplicationId);
            var fee = app.CalculateFee();

            var rrr = await remita.GenerateRRRAsync(
                app.Id, fee, app.CompanyName, app.Email, app.PhoneNumber);

            app.SetRRR(rrr);
            await repository.UpdateAsync(app);

            return new GenerateRRRResponse(rrr, fee, app.Id);
        }

        public async Task<SubmitApplicationResponse> SubmitAsync(SubmitApplicationRequest req)
        {
            var app = await GetOrThrowAsync(req.ApplicationId);
            app.Submit();
            await repository.UpdateAsync(app);
            return new SubmitApplicationResponse(app.Id, app.Status, app.UpdatedAt!.Value);
        }

        private async Task<Domain.Entities.Application> GetOrThrowAsync(Guid id)
        {
            var app = await repository.GetByIdAsync(id);
            if (app is null) throw new KeyNotFoundException($"Application {id} not found");
            return app;
        }
    }
}
