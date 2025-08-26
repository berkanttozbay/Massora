using AutoMapper;
using Humanizer;
using Massora.Business.DTOs;
using Massora.Common.Repositories;
using Massora.DataAccess.Context;
using Massora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace Massora.Business.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IRepository<Vehicle> _repository;
        private readonly IMapper _mapper;

        public VehicleService(IRepository<Vehicle> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle, string userId)
        {
            // 1. Kullanýcý ID'si ile Auth veritabanýndan companyId'yi bul
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;

            
            if (user == null || user.CompanyId == null)
            {
                // Kullanýcý bulunamadý veya bir þirkete atanmamýþsa hata fýrlat
                throw new Exception("Kullanýcý bir þirkete atanmamýþ.");
            }

            // 2. Yeni Vehicle entity'sini oluþtur
            var newVehicle = new Vehicle
            {
                // DTO'dan gelen veriler 
                LicensePlate = vehicle.LicensePlate,
                VehicleType = vehicle.VehicleType,
                HourlyWageDriver = vehicle.HourlyWageDriver,
                HourlyWagePartner = vehicle.HourlyWagePartner,

                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullanýn
            };

            // Aracýn eklenmesi
            return await _repository.AddAsync(newVehicle);
;
            
        }

        public async Task<Vehicle> UpdateAsync(int id, string userId, Vehicle vehicle)
        {
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;
            // 1. Önce güncellenecek kaydý veritabanýnda bul.
            var entityToUpdate = await _repository.GetByIdAsync(id);
            if (entityToUpdate == null)
            {
                throw new Exception("Güncellenecek kayýt bulunamadý.");
            }


            // DTO'dan gelen veriler 
            entityToUpdate.LicensePlate = vehicle.LicensePlate;
            entityToUpdate.VehicleType = vehicle.VehicleType;
            entityToUpdate.HourlyWageDriver = vehicle.HourlyWageDriver;
            entityToUpdate.HourlyWagePartner = vehicle.HourlyWagePartner;

            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId; // companyId nullable ise .Value kullanýn
            

            // 4. Repository aracýlýðýyla güncelle ve deðiþiklikleri kaydet.
            return await _repository.UpdateAsync(entityToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetByCompanyIdAsync(int companyId)
        {
            return await _repository.GetWhereAsync(x => x.CompanyId == companyId);
        }

        public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
        {
            var vehicles = await _repository.GetWhereAsync(x => x.LicensePlate == licensePlate);
            return vehicles.FirstOrDefault();
        }
        public async Task<PaginationResultModel<VehicleDto>> GetVehiclesPaginatedAsync(string loggedInUserId,int pageNumber, int pageSize, string searchTerm)
        {
                
            
            var query = _repository.GetAsQueryable();

            query = query.Where(item => item.Company.ResponsibleUserId.Equals(loggedInUserId) || item.Drivers.Any(d=>d.ResponsibleUserId.Equals(loggedInUserId)));
            query = _repository.GetAsQueryable()
                .Include(driver => driver.Company);
            // ARAMA FÝLTRESÝ
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(v =>
                    v.VehicleType.ToLower().Contains(term) ||
                    v.LicensePlate.ToLower().Contains(term)

                );
            }

            // TOPLAM KAYIT SAYISINI ALMA

            var totalCount = await query.CountAsync();

            // SAYFALAMA VE VERÝYÝ ÇEKME

            var vehicles = await query
                .OrderBy(d => d.VehicleType) // Tutarlý bir sýralama için OrderBy eklemek önemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VERÝYÝ DTO'YA DÖNÜÞTÜRME (MAP'LEME)
            var vehiclesDtos = _mapper.Map<List<VehicleDto>>(vehicles);

            // SONUCU OLUÞTURMA
            var result = new PaginationResultModel<VehicleDto>
            {
                Items = vehiclesDtos,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return result;
        }
        public async Task<IEnumerable<DropDownDto>> GetVehiclesForDropdownAsync(string userId)
        {
            // 2. Önce kullanýcýyý ve þirketini DOÐRU mantýkla buluyoruz.
            // Bu sorgu veritabanýna gider, tüm kullanýcýlarý belleðe çekmez.
            var user = await _repository.GetAsQueryable() // Veya doðrudan DbContext
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Company.ResponsibleUserId.Equals(userId));

            if (user == null || user.CompanyId == null)
            {
                // Kullanýcý yoksa veya þirkete atanmamýþsa boþ liste döndür.
                return new List<DropDownDto>();
            }

            var companyId = user.CompanyId;

            // 3. Araçlarý, veritabaný seviyesinde filtreleyip istediðimiz formata dönüþtürüyoruz.
            // _repository'nin VehicleRepository olduðunu varsayýyorum.
            return await _repository.GetAsQueryable()
                .Where(v => v.CompanyId.Equals(companyId) && !v.IsDeleted) // Önce filtrele
                .Select(v => new DropDownDto { Id = v.Id, Name = v.LicensePlate }) // Sonra seç
                .ToListAsync(); // En son listeye çevir

        }
    }
} 