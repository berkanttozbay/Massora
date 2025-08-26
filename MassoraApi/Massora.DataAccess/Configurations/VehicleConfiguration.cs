using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Massora.Domain.Entities;

namespace Massora.DataAccess.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.CompanyId).IsRequired();
            builder.Property(x => x.VehicleType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.HourlyWageDriver).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.HourlyWagePartner).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.LicensePlate).IsRequired().HasMaxLength(20);

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

            // Foreign Key
            builder.HasOne(x => x.Company)
                   .WithMany(x => x.Vehicles)   
                   .HasForeignKey(x => x.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Indexes
            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.LicensePlate).IsUnique();
            builder.HasIndex(x => x.VehicleType);
        }
    }
} 