using AutoMapper;
using Massora.Business.DTOs;
using Massora.Common.Repositories;
using Massora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Massora.Business.Services
{
    public class WorkHistoryService : IWorkHistoryService
    {
        private readonly IRepository<WorkHistory> _repository;
        private readonly IMapper _mapper;

        public WorkHistoryService(IRepository<WorkHistory> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<WorkHistory> AddAsync(WorkHistory workHistory,string userId)
        {
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;

            if (user == null || user.CompanyId == null)
            {
                // Kullan�c� bulunamad� veya bir �irkete atanmam��sa hata f�rlat
                throw new Exception("Kullan�c� bir �irkete atanmam��.");
            }

            // 2. Yeni Vehicle entity'sini olu�tur
            var newWorkHistory = new WorkHistory
            {
                // DTO'dan gelen veriler
                PartnerCompanyId = workHistory.PartnerCompanyId,
                Date = workHistory.Date,
                StartTime = workHistory.StartTime,
                EndTime = workHistory.EndTime,
                CalculatedDriverFee = workHistory.CalculatedDriverFee,
                CalculatedPartnerFee = workHistory.CalculatedPartnerFee,
                Address = workHistory.Address,
                DriverId = workHistory.DriverId,
                VehicleId = workHistory.VehicleId,


                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullan�n
            };

            // Arac�n eklenmesi
            return await _repository.AddAsync(newWorkHistory);
        }

        public async Task<WorkHistory> UpdateAsync(int id, string userId, WorkHistory workHistory)
        {
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;
            // 1. �nce g�ncellenecek kayd� veritaban�nda bul.
            var entityToUpdate = await _repository.GetByIdAsync(id);
            if (entityToUpdate == null)
            {
                throw new Exception("G�ncellenecek kay�t bulunamad�.");
            }


            // DTO'dan gelen veriler
            entityToUpdate.PartnerCompanyId = workHistory.PartnerCompanyId;
            entityToUpdate.Date = workHistory.Date;
            entityToUpdate.StartTime = workHistory.StartTime;
            entityToUpdate.EndTime = workHistory.EndTime;
            entityToUpdate.CalculatedDriverFee = workHistory.CalculatedDriverFee;
            entityToUpdate.CalculatedPartnerFee = workHistory.CalculatedPartnerFee;
            entityToUpdate.Address = workHistory.Address;
            entityToUpdate.DriverId = workHistory.DriverId; ;


            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId;// companyId nullable ise .Value kullan�n
            

            // 4. Repository arac�l���yla g�ncelle ve de�i�iklikleri kaydet.
            return await _repository.UpdateAsync(entityToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<WorkHistory?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WorkHistory>> GetAllAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<IEnumerable<WorkHistory>> GetByDriverIdAsync(int driverId)
        {
            return await _repository.GetWhereAsync(x => x.DriverId == driverId);
        }

        public async Task<IEnumerable<WorkHistory>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _repository.GetWhereAsync(x => x.VehicleId == vehicleId);
        }

        public async Task<IEnumerable<WorkHistory>> GetByCompanyIdAsync(int companyId)
        {
            return await _repository.GetWhereAsync(x => x.CompanyId == companyId);
        }

        public async Task<IEnumerable<WorkHistory>> GetByPartnerCompanyIdAsync(int partnerCompanyId)
        {
            return await _repository.GetWhereAsync(x => x.PartnerCompanyId == partnerCompanyId);
        }

        public async Task<IEnumerable<WorkHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _repository.GetWhereAsync(x => x.Date >= startDate && x.Date <= endDate);
        }

        public async Task<WorkHistory?> GetActiveWorkByDriverIdAsync(int driverId)
        {
            var activeWorks = await _repository.GetWhereAsync(x => x.DriverId == driverId && x.EndTime == null);
            return activeWorks.FirstOrDefault();
        }
        public async Task<PaginationResultModel<WorkHistoryDto>> GetWorkHistoriesPaginatedAsync(string loggedInUserId, int pageNumber, int pageSize, string searchTerm)
        {
            var query = _repository.GetAsQueryable();

            query = query.Where(item => item.Company.ResponsibleUserId.Equals(loggedInUserId) || item.Driver.ResponsibleUserId.Equals(loggedInUserId));
            query = _repository.GetAsQueryable()
                .Include(driver => driver.Company)
                .Include(driver => driver.Vehicle)
                .Include(driver => driver.Driver);
            // ARAMA F�LTRES�
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(v =>
                    v.Address.ToLower().Contains(term) ||
                    v.DriverId.ToString().ToLower().Contains(term)

                );
            }

            // TOPLAM KAYIT SAYISINI ALMA

            var totalCount = await query.CountAsync();

            // SAYFALAMA VE VER�Y� �EKME

            var workHistories = await query
                .OrderBy(d => d.Address) // Tutarl� bir s�ralama i�in OrderBy eklemek �nemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VER�Y� DTO'YA D�N��T�RME (MAP'LEME)
            var workHistoryDtos = _mapper.Map<List<WorkHistoryDto>>(workHistories);

            // SONUCU OLU�TURMA
            var result = new PaginationResultModel<WorkHistoryDto>
            {
                Items = workHistoryDtos,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return result;
        }
    }
}