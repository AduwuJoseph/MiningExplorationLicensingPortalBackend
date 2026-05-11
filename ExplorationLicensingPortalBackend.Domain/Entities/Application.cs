using ExplorationLicensingPortalBackend.Domain.Enums;

namespace ExplorationLicensingPortalBackend.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; private set; }
        public string CompanyName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Address { get; private set; } = null!;
        public string ContactPersonName { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string State { get; private set; } = null!;
        public string LocalGovernmentArea { get; private set; } = null!;
        public ApplicationType ApplicationType { get; private set; }
        public string MineralTypes { get; private set; } = null!;       // comma-separated mineral types
        public string? ExportCountry { get; private set; }     // for export permits
        public int? CadastreUnits { get; private set; }        // for mining/exploration leases
        public string? RRR { get; private set; }               // Remita Retrieval Reference
        public ApplicationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Payment? Payment { get; private set; }
        public List<Document> Documents { get; private set; } = new();

        private Application() { }

        public static Application Create(
            string companyName, string email, string address,
            string contactPersonName, string phoneNumber,
            string state, string localGovernmentArea,
            ApplicationType applicationType, string mineralTypes,
            string? exportCountry = null, int? cadastreUnits = null)
        {
            if (string.IsNullOrWhiteSpace(companyName)) throw new ArgumentException("Company name is required");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(mineralTypes)) throw new ArgumentException("Mineral type(s) are required");

            return new Application
            {
                Id = Guid.NewGuid(),
                CompanyName = companyName,
                Email = email,
                Address = address,
                ContactPersonName = contactPersonName,
                PhoneNumber = phoneNumber,
                State = state,
                LocalGovernmentArea = localGovernmentArea,
                ApplicationType = applicationType,
                MineralTypes = mineralTypes,
                ExportCountry = exportCountry,
                CadastreUnits = cadastreUnits,
                Status = ApplicationStatus.Initiated,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void SetRRR(string rrr)
        {
            if (string.IsNullOrWhiteSpace(rrr)) throw new ArgumentException("RRR is required");
            RRR = rrr;
            Status = ApplicationStatus.RRRGenerated;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddPayment(Payment payment)
        {
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
            Status = ApplicationStatus.PaymentCompleted;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            Documents.Add(document);
            Status = ApplicationStatus.DocumentsUploaded;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Submit()
        {
            if (Status != ApplicationStatus.DocumentsUploaded)
                throw new InvalidOperationException("Documents must be uploaded before submission");
            Status = ApplicationStatus.Submitted;
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal CalculateFee()
        {
            return ApplicationType switch
            {
                ApplicationType.ExplorationLicence => 500000m + (50000m * ((CadastreUnits ?? 1) - 1)),
                ApplicationType.QuarryLease => 600000m + (250000m * ((CadastreUnits ?? 1) - 1)),
                ApplicationType.SmallScaleMiningLease => 300000m + (200000m * ((CadastreUnits ?? 1) - 1)),
                ApplicationType.LicenceToPurchaseAndPossess => CalculatePurchasePossessFee(),
                ApplicationType.PermitToExportCommercial => 100000m,
                ApplicationType.PermitToExportSamples => 10000m,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private decimal CalculatePurchasePossessFee()
        {
            // Fee is per mineral type; caller passes comma-separated minerals
            // Pricing: NonMetallic/Metallic = 100,000; Gemstones/PreciousMetals = 200,000
            // Without mineral category info here, return base 100,000 per mineral as default
            var count = MineralTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).Length;
            return 100000m * count;
        }
    }
}
