using ExplorationLicensingPortalBackend.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ExplorationLicensingPortalBackend.Infrastructure.Services
{
    public class RemitaService(HttpClient httpClient, IConfiguration config) : IRemitaService
    {
        private readonly string _merchantId = config["Remita:MerchantId"]!;
        private readonly string _serviceTypeId = config["Remita:ServiceTypeId"]!;
        private readonly string _apiKey = config["Remita:ApiKey"]!;

        public async Task<string> GenerateRRRAsync(Guid applicationId, decimal amount, string payerName, string payerEmail, string payerPhone)
        {
            var orderId = applicationId.ToString("N");
            var hash = ComputeHash($"{_merchantId}{_serviceTypeId}{orderId}{amount}{_apiKey}");

            var payload = new
            {
                serviceTypeId = _serviceTypeId,
                amount = amount.ToString("F2"),
                orderId,
                payerName,
                payerEmail,
                payerPhone,
                description = "Mining Application Fee"
            };

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"remitaConsumerKey={_merchantId},remitaConsumerToken={hash}");

            var response = await httpClient.PostAsync(
                $"merchant/api/v1/merchant/init",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("RRR").GetString()!;
        }

        public async Task<bool> VerifyPaymentAsync(string rrr)
        {
            var hash = ComputeHash($"{rrr}{_apiKey}{_merchantId}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"remitaConsumerKey={_merchantId},remitaConsumerToken={hash}");

            var response = await httpClient.GetAsync($"merchant/api/v1/merchant/payment/query/{rrr}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("status").GetString() == "01";
        }

        private static string ComputeHash(string input)
        {
            var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
