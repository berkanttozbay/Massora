namespace Massora.Domain.Entities
{
    public class Driver : BaseEntity
    {
        public string ResponsibleUserId { get; set; }
        public int CompanyId { get; set; }
        public int? VehicleId { get; set; } // todo
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();
        public virtual ICollection<VehicleFuelHistory> FuelHistories { get; set; } = new List<VehicleFuelHistory>();
    }
} 