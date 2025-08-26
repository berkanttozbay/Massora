using Massora.Business.DTOs;
using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public interface IVehicleFuelHistoryService
    {
        Task<VehicleFuelHistory> AddAsync(VehicleFuelHistory fuelHistory, string userId);
        Task<VehicleFuelHistory> UpdateAsync(int id,string userId,VehicleFuelHistory fuelHistory);
        Task DeleteAsync(int id);
        Task<VehicleFuelHistory?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleFuelHistory>> GetAllAsync();
        Task<IEnumerable<VehicleFuelHistory>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleFuelHistory>> GetByDriverIdAsync(int driverId);
        Task<IEnumerable<VehicleFuelHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<PaginationResultModel<VehicleFuelHistoryDto>> GetVehicleFuelHistoriesPaginatedAsync(string loggedInUserId, int pageNumber, int pageSize, string searchTerm);
    }
} 