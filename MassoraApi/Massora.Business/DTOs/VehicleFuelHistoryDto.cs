namespace Massora.Business.DTOs
{
    public class VehicleFuelHistoryDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string FuelCompany { get; set; } = string.Empty;
        public decimal Liter { get; set; }
        public decimal Fee { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateVehicleFuelHistoryDto
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public string FuelCompany { get; set; } = string.Empty;
        public decimal Liter { get; set; }
        public decimal Fee { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
    }

    public class UpdateVehicleFuelHistoryDto
    {
        public int VehicleId { get; set; } 
        public int DriverId { get; set; }
        public string FuelCompany { get; set; } = string.Empty;
        public decimal Liter { get; set; }
        public decimal Fee { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
    }
} 