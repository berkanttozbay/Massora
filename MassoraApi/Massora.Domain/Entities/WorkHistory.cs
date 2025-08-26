namespace Massora.Domain.Entities
{
    public class WorkHistory : BaseEntity
    {
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public int CompanyId { get; set; }
        public int PartnerCompanyId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal CalculatedDriverFee { get; set; }
        public decimal CalculatedPartnerFee { get; set; }
        public string Address { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Driver Driver { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual PartnerCompany PartnerCompany { get; set; } = null!;
    }
} 