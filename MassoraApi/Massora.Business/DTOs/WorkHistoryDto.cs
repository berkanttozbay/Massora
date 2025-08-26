namespace Massora.Business.DTOs
{
    public class WorkHistoryDto
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int PartnerCompanyId { get; set; }
        public string PartnerCompanyName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal CalculatedDriverFee { get; set; }
        public decimal CalculatedPartnerFee { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateWorkHistoryDto
    {
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public int CompanyId { get; set; }
        public decimal CalculatedDriverFee { get; set; }
        public decimal CalculatedPartnerFee { get; set; }
        public int PartnerCompanyId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Address { get; set; } = string.Empty;
    }

    public class UpdateWorkHistoryDto
    {
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public DateTime? EndTime { get; set; }
        public int PartnerCompanyId { get; set; }
        public decimal CalculatedDriverFee { get; set; }
        public decimal CalculatedPartnerFee { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
    }
} 