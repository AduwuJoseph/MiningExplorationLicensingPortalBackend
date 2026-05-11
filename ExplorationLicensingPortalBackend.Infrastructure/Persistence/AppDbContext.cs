using ExplorationLicensingPortalBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExplorationLicensingPortalBackend.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Application>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Email).HasMaxLength(150).IsRequired();
                e.Property(x => x.MineralTypes).HasMaxLength(500).IsRequired();
                e.Property(x => x.ExportCountry).HasMaxLength(100);
                e.Property(x => x.RRR).HasMaxLength(50);
                e.HasMany(x => x.Documents).WithOne().HasForeignKey(d => d.ApplicationId);
                e.HasOne(x => x.Payment).WithOne().HasForeignKey<Payment>(p => p.ApplicationId);
            });

            modelBuilder.Entity<Document>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FileName).HasMaxLength(300).IsRequired();
                e.Property(x => x.BlobUrl).HasMaxLength(1000).IsRequired();
            });

            modelBuilder.Entity<Payment>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.TransactionReference).HasMaxLength(100).IsRequired();
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
