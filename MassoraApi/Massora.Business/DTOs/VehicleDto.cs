namespace Massora.Business.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public decimal HourlyWageDriver { get; set; }
        public decimal HourlyWagePartner { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateVehicleDto
    {
        public int CompanyId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public decimal HourlyWageDriver { get; set; }
        public decimal HourlyWagePartner { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
    }

    public class UpdateVehicleDto
    {
        public string VehicleType { get; set; } = string.Empty;
        public decimal HourlyWageDriver { get; set; }
        public decimal HourlyWagePartner { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
    }
} 