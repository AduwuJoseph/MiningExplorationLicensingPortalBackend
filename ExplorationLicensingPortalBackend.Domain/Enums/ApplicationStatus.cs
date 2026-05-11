using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorationLicensingPortalBackend.Domain.Enums
{
    public enum ApplicationStatus
    {
        Initiated = 1,
        PaymentPending = 2,
        RRRGenerated = 3,
        PaymentCompleted = 4,
        DocumentsUploaded = 5,
        Submitted = 6,
        UnderReview = 7,
        Approved = 8,
        Rejected = 9
    }
}
