using ExplorationLicensingPortalBackend.Domain.Interfaces;
using ExplorationLicensingPortalBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExplorationLicensingPortalBackend.Infrastructure.Repositories
{
    public class ApplicationRepository(AppDbContext db) : IApplicationRepository
    {
        public async Task<Domain.Entities.Application?> GetByIdAsync(Guid id) =>
            await db.Applications.Include(a => a.Documents).Include(a => a.Payment)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Domain.Entities.Application> CreateAsync(Domain.Entities.Application application)
        {
            db.Applications.Add(application);
            await db.SaveChangesAsync();
            return application;
        }

        public async Task UpdateAsync(Domain.Entities.Application application)
        {
            db.Applications.Update(application);
            await db.SaveChangesAsync();
        }
    }
}
