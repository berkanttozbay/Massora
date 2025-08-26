namespace Massora.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string ResponsibleUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
        public virtual ICollection<PartnerCompany> PartnerCompanies { get; set; } = new List<PartnerCompany>();
        public virtual ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();
        public virtual ICollection<VehicleFuelHistory> FuelHistories { get; set; } = new List<VehicleFuelHistory>();

    }
} 