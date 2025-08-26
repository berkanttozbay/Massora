namespace Massora.Domain.Entities
{
    public class PartnerCompany : BaseEntity
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();
    }
} 