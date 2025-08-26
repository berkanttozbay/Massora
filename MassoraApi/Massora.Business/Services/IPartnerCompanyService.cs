using Massora.Business.DTOs;
using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public interface IPartnerCompanyService
    {
        Task<PartnerCompany> AddAsync(PartnerCompany partnerCompany,string id);
        Task<PartnerCompany> UpdateAsync(int id , string userId,PartnerCompany partnerCompany);
        Task DeleteAsync(int id);
        Task<PartnerCompany?> GetByIdAsync(int id);
        Task<IEnumerable<PartnerCompany>> GetAllAsync();
        Task<IEnumerable<PartnerCompany>> GetByCompanyIdAsync(int companyId);
        Task<PaginationResultModel<PartnerCompanyDto>> GetPartnerCompaniesPaginatedAsync(string loggedInUserId,int pageNumber, int pageSize, string searchTerm);
        Task<IEnumerable<DropDownDto>> GetPartnerCompaniesForDropdownAsync(string userId);

    }
} 