using ExplorationLicensingPortalBackend.Domain.Entities;

namespace ExplorationLicensingPortalBackend.Domain.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Entities.Application?> GetByIdAsync(Guid id);
        Task<Entities.Application> CreateAsync(Entities.Application application);
        Task UpdateAsync(Entities.Application application);
    }
}
