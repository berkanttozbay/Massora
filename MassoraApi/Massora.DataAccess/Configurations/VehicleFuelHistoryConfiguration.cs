using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Massora.Domain.Entities;

namespace Massora.DataAccess.Configurations
{
    public class VehicleFuelHistoryConfiguration : IEntityTypeConfiguration<VehicleFuelHistory>
    {
        public void Configure(EntityTypeBuilder<VehicleFuelHistory> builder)
        {
            builder.ToTable("VehicleFuelHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.VehicleId).IsRequired();
            builder.Property(x => x.DriverId).IsRequired();
            builder.Property(x => x.FuelCompany).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Liter).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Fee).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Time).IsRequired();

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

            // Foreign Keys
            builder.HasOne(x => x.Vehicle)
                   .WithMany(x => x.FuelHistories)
                   .HasForeignKey(x => x.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Driver)
                   .WithMany(x => x.FuelHistories)
                   .HasForeignKey(x => x.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
                   .WithMany(x => x.FuelHistories)
                   .HasForeignKey(x => x.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.DriverId);
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.FuelCompany);
        }
    }
} 