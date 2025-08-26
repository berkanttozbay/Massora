using Massora.Business.DTOs;
using Massora.Common.Repositories;
using Massora.DataAccess.Repositories;
using Massora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massora.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Vehicle> _vehicleRepo;
        private readonly IRepository<Driver> _driverRepo;
        private readonly IRepository<PartnerCompany> _partnerRepo;
        private readonly IRepository<WorkHistory> _workHistoryRepo;

        public DashboardService(
            IRepository<Vehicle> vehicleRepo,
            IRepository<Driver> driverRepo,
            IRepository<PartnerCompany> partnerRepo,
            IRepository<WorkHistory> workHistoryRepo)
        {
            _vehicleRepo = vehicleRepo;
            _driverRepo = driverRepo;
            _partnerRepo = partnerRepo;
            _workHistoryRepo = workHistoryRepo;
        }

        public async Task<DashboardStatsDto> GetStatsForUserAsync(string userId)
        {
            // Önce kullanıcının CompanyId'sini bul
            var users = _vehicleRepo.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;
            if (user?.CompanyId == null)
            {
                throw new Exception("Kullanıcı bir şirkete atanmamış.");
            }

            // Her bir entity için, o şirkete ait kayıtların sayısını al
            var vehicleCount = await _vehicleRepo.CountAsync(v => v.CompanyId == companyId && !v.IsDeleted);
            var driverCount = await _driverRepo.CountAsync(d => d.CompanyId == companyId && !d.IsDeleted);
            var partnerCount = await _partnerRepo.CountAsync(p => p.CompanyId == companyId && !p.IsDeleted);
            var workHistoryCount = await _workHistoryRepo.CountAsync(wh => wh.CompanyId == companyId && !wh.IsDeleted); // Örnek

            // DTO'yu doldur ve döndür
            return new DashboardStatsDto
            {
                VehicleCount = vehicleCount,
                DriverCount = driverCount,
                PartnerCompanyCount = partnerCount,
                ActiveWorkHistoryCount = workHistoryCount
            };
        }
    }
}
