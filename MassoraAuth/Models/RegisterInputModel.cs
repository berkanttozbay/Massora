using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "Driver123*"; // Varsayılan şifre
    }
}
