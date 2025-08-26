using Microsoft.EntityFrameworkCore;
using Massora.Domain.Entities;
using Massora.DataAccess.Configurations;

namespace Massora.DataAccess.Context
{
    public class MassoraDbContext : DbContext
    {
        public MassoraDbContext(DbContextOptions<MassoraDbContext> options) : base(options)
        {
        }

        
        public DbSet<Company> Companies { get; set; }
        public DbSet<PartnerCompany> PartnerCompanies { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<WorkHistory> WorkHistories { get; set; }
        public DbSet<VehicleFuelHistory> VehicleFuelHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Entity configurations todo bunlarýn dinamik yapýsýný yap
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new PartnerCompanyConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new DriverConfiguration());
            modelBuilder.ApplyConfiguration(new WorkHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleFuelHistoryConfiguration());
        }
    }
} 