using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorationLicensingPortalBackend.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid ApplicationId { get; private set; }
        public decimal Amount { get; private set; }
        public string TransactionReference { get; private set; } = null!;
        public DateTime PaymentDate { get; private set; }
        public PaymentStatus Status { get; private set; }

        private Payment() { }

        public static Payment Create(Guid applicationId, decimal amount, string transactionReference)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0");
            if (string.IsNullOrWhiteSpace(transactionReference))
                throw new ArgumentException("Transaction reference is required");

            return new Payment
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                Amount = amount,
                TransactionReference = transactionReference,
                PaymentDate = DateTime.UtcNow,
                Status = PaymentStatus.Completed
            };
        }
    }
}
