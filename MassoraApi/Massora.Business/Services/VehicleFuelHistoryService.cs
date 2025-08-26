using AutoMapper;
using Massora.Business.DTOs;
using Massora.Common.Repositories;
using Massora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Massora.Business.Services
{
    public class VehicleFuelHistoryService : IVehicleFuelHistoryService
    {
        private readonly IRepository<VehicleFuelHistory> _repository;
        private readonly IMapper _mapper;

        public VehicleFuelHistoryService(IRepository<VehicleFuelHistory> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<VehicleFuelHistory> AddAsync(VehicleFuelHistory fuelHistory,string userId)
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
            var newVehicleFuelHistory = new VehicleFuelHistory
            {
                // DTO'dan gelen veriler
                FuelCompany = fuelHistory.FuelCompany,
                Liter = fuelHistory.Liter,
                Fee = fuelHistory.Fee,
                Date = fuelHistory.Date,
                Time = fuelHistory.Time,
                VehicleId = fuelHistory.VehicleId,
                DriverId = fuelHistory.DriverId,


                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullan�n
            };

            // Arac�n eklenmesi
            return await _repository.AddAsync(newVehicleFuelHistory);
        }

        public async Task<VehicleFuelHistory> UpdateAsync(int id , string userId,VehicleFuelHistory fuelHistory)
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
            entityToUpdate.FuelCompany = fuelHistory.FuelCompany;
            entityToUpdate.Liter = fuelHistory.Liter;
            entityToUpdate.Fee = fuelHistory.Fee;
            entityToUpdate.Date = fuelHistory.Date;
            entityToUpdate.Time = fuelHistory.Time;
            entityToUpdate.VehicleId = fuelHistory.VehicleId;
            entityToUpdate.DriverId = fuelHistory.DriverId;


            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId;// companyId nullable ise .Value kullan�n
            

            // 4. Repository arac�l���yla g�ncelle ve de�i�iklikleri kaydet.
            return await _repository.UpdateAsync(entityToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<VehicleFuelHistory?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<VehicleFuelHistory>> GetAllAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<IEnumerable<VehicleFuelHistory>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _repository.GetWhereAsync(x => x.VehicleId == vehicleId);
        }

        public async Task<IEnumerable<VehicleFuelHistory>> GetByDriverIdAsync(int driverId)
        {
            return await _repository.GetWhereAsync(x => x.DriverId == driverId);
        }

        public async Task<IEnumerable<VehicleFuelHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _repository.GetWhereAsync(x => x.Date >= startDate && x.Date <= endDate);
        }
        public async Task<PaginationResultModel<VehicleFuelHistoryDto>> GetVehicleFuelHistoriesPaginatedAsync(string loggedInUserId, int pageNumber, int pageSize, string searchTerm)
        {
            var query = _repository.GetAsQueryable();
            query = query.Where(item => item.Company.ResponsibleUserId.Equals(loggedInUserId) || item.Driver.ResponsibleUserId.Equals(loggedInUserId));
            query = _repository.GetAsQueryable()
                .Include(driver => driver.Company)
                .Include(driver => driver.Vehicle)
                .Include(driver => driver.Driver); // <-- EKS�K OLAN KR�T�K SATIR
            // ARAMA F�LTRES�
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(v =>
                    v.FuelCompany.ToLower().Contains(term) 
                    
                );
            }

            // TOPLAM KAYIT SAYISINI ALMA

            var totalCount = await query.CountAsync();

            // SAYFALAMA VE VER�Y� �EKME

            var vehicleFuelHistories = await query
                .OrderBy(d => d.VehicleId) // Tutarl� bir s�ralama i�in OrderBy eklemek �nemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VER�Y� DTO'YA D�N��T�RME (MAP'LEME)
            var vehicleFuelHistoryDtos = _mapper.Map<List<VehicleFuelHistoryDto>>(vehicleFuelHistories);

            // SONUCU OLU�TURMA
            var result = new PaginationResultModel<VehicleFuelHistoryDto>
            {
                Items = vehicleFuelHistoryDtos,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return result;
        }
        
    }
} 