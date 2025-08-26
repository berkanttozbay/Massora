using Massora.Business.DTOs;
using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> AddAsync(Vehicle vehicle, string userId);
        Task<Vehicle> UpdateAsync(int id,string userId,Vehicle vehicle);
        Task DeleteAsync(int id);
        Task<Vehicle?> GetByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<IEnumerable<Vehicle>> GetByCompanyIdAsync(int companyId);
        Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
        Task<PaginationResultModel<VehicleDto>> GetVehiclesPaginatedAsync(string loggedInUserId,int pageNumber, int pageSize, string searchTerm);
        Task<IEnumerable<DropDownDto>> GetVehiclesForDropdownAsync(string userId);

    }
} 