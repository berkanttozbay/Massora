using Massora.Business.DTOs;
using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public interface IDriverService
    {
        Task<Driver> AddAsync(Driver driver,string userId);
        Task<Driver> UpdateAsync(int id,string userId,Driver driver);
        Task DeleteAsync(int id);
        Task<Driver?> GetByIdAsync(int id);
        Task<IEnumerable<Driver>> GetAllAsync();
        Task<IEnumerable<Driver>> GetByCompanyIdAsync(int companyId);
        Task<Driver?> GetByEmailAsync(string email);
        Task<Driver?> GetByPhoneAsync(string phone);
        Task<PaginationResultModel<DriverDto>> GetDriversPaginatedAsync(string loggedInUserId, int pageNumber, int pageSize, string searchTerm);
        Task<IEnumerable<DropDownDto>> GetDriversForDropdownAsync(string userId);
    }
} 