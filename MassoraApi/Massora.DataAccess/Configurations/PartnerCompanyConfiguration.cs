using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Massora.Domain.Entities;

namespace Massora.DataAccess.Configurations
{
    public class PartnerCompanyConfiguration : IEntityTypeConfiguration<PartnerCompany>
    {
        public void Configure(EntityTypeBuilder<PartnerCompany> builder)
        {
            builder.ToTable("PartnerCompanies");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.CompanyId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ContactPhone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ContactEmail).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(500);

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

            // Foreign Key
            builder.HasOne(x => x.Company)
                   .WithMany(x => x.PartnerCompanies)
                   .HasForeignKey(x => x.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.Name);
        }
    }
} 