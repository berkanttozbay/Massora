using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Massora.Domain.Entities;

namespace Massora.DataAccess.Configurations
{
    public class WorkHistoryConfiguration : IEntityTypeConfiguration<WorkHistory>
    {
        public void Configure(EntityTypeBuilder<WorkHistory> builder)
        {
            builder.ToTable("WorkHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.DriverId).IsRequired();
            builder.Property(x => x.VehicleId).IsRequired();
            builder.Property(x => x.CompanyId).IsRequired();
            builder.Property(x => x.PartnerCompanyId).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime);
            builder.Property(x => x.CalculatedDriverFee).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.CalculatedPartnerFee).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Address).IsRequired().HasMaxLength(500);

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

            // Foreign Keys
            builder.HasOne(x => x.Driver)
                   .WithMany(x => x.WorkHistories)
                   .HasForeignKey(x => x.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vehicle)
                   .WithMany(x => x.WorkHistories)
                   .HasForeignKey(x => x.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
                   .WithMany(x => x.WorkHistories)
                   .HasForeignKey(x => x.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PartnerCompany)
                   .WithMany(x => x.WorkHistories)
                   .HasForeignKey(x => x.PartnerCompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.DriverId);
            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.PartnerCompanyId);
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.StartTime);
        }
    }
} 