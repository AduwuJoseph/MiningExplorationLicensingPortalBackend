namespace ExplorationLicensingPortalBackend.Domain.Interfaces
{
    public interface IRemitaService
    {
        Task<string> GenerateRRRAsync(Guid applicationId, decimal amount, string payerName, string payerEmail, string payerPhone);
        Task<bool> VerifyPaymentAsync(string rrr);
    }
}
