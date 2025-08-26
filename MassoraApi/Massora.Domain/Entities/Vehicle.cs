namespace Massora.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public int CompanyId { get; set; }
        public string VehicleType { get; set; } = string.Empty; //to do  Kepçe, Kamyon vb.enum
        public decimal HourlyWageDriver { get; set; }
        public decimal HourlyWagePartner { get; set; }
        public string LicensePlate { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();
        public virtual ICollection<VehicleFuelHistory> FuelHistories { get; set; } = new List<VehicleFuelHistory>();
        public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    }
} 