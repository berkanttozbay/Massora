using Massora.Business.DTOs;
using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public interface IWorkHistoryService
    {
        Task<WorkHistory> AddAsync(WorkHistory workHistory,string userId);
        Task<WorkHistory> UpdateAsync(int id ,string userId,WorkHistory workHistory);
        Task DeleteAsync(int id);
        Task<WorkHistory?> GetByIdAsync(int id);
        Task<IEnumerable<WorkHistory>> GetAllAsync();
        Task<IEnumerable<WorkHistory>> GetByDriverIdAsync(int driverId);
        Task<IEnumerable<WorkHistory>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<WorkHistory>> GetByCompanyIdAsync(int companyId);
        Task<IEnumerable<WorkHistory>> GetByPartnerCompanyIdAsync(int partnerCompanyId);
        Task<IEnumerable<WorkHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<WorkHistory?> GetActiveWorkByDriverIdAsync(int driverId);
        Task<PaginationResultModel<WorkHistoryDto>> GetWorkHistoriesPaginatedAsync(string loggedInUserId, int pageNumber, int pageSize, string searchTerm);
    }
} 