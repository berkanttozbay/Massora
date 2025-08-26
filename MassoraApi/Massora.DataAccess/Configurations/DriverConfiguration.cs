using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Massora.Domain.Entities;

namespace Massora.DataAccess.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.ResponsibleUserId).IsRequired();
            builder.Property(x => x.CompanyId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
            builder.Property(x => x.BirthDate).IsRequired();
            builder.Property(x => x.Gender).IsRequired().HasMaxLength(10);

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.VehicleId).IsRequired(false); 

            // Foreign Key
            builder.HasOne(x => x.Company)
                   .WithMany(x => x.Drivers)
                   .HasForeignKey(x => x.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vehicle)
                   .WithMany(x => x.Drivers)
                   .HasForeignKey(x => x.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone);
            builder.HasIndex(x => x.Name);
        }
    }
} 