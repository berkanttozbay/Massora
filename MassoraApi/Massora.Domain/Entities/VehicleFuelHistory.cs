namespace Massora.Domain.Entities
{
    public class VehicleFuelHistory : BaseEntity
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public int CompanyId { get; set; }
        public string FuelCompany { get; set; } = string.Empty;
        public decimal Liter { get; set; }
        public decimal Fee { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }

        // Navigation Properties
        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual Driver Driver { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
} 