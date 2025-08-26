namespace Massora.Business.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string ResponsibleUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateCompanyDto
    {
        public string ResponsibleUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class UpdateCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
} 