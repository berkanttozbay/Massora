using System.ComponentModel.DataAnnotations;

namespace Massora.Business.DTOs
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string ResponsibleUserId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int? VehicleId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
    }

    public class CreateDriverDto
    {
        public int CompanyId { get; set; }
        public int VehicleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        // Giriþ Bilgileri
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; } = "Driver123*";
    }

    public class UpdateDriverDto
    {
        public int VehicleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 