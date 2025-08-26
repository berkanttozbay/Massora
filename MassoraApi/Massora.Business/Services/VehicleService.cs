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
            // 1. Kullan�c� ID'si ile Auth veritaban�ndan companyId'yi bul
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;

            
            if (user == null || user.CompanyId == null)
            {
                // Kullan�c� bulunamad� veya bir �irkete atanmam��sa hata f�rlat
                throw new Exception("Kullan�c� bir �irkete atanmam��.");
            }

            // 2. Yeni Vehicle entity'sini olu�tur
            var newVehicle = new Vehicle
            {
                // DTO'dan gelen veriler 
                LicensePlate = vehicle.LicensePlate,
                VehicleType = vehicle.VehicleType,
                HourlyWageDriver = vehicle.HourlyWageDriver,
                HourlyWagePartner = vehicle.HourlyWagePartner,

                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullan�n
            };

            // Arac�n eklenmesi
            return await _repository.AddAsync(newVehicle);
;
            
        }

        public async Task<Vehicle> UpdateAsync(int id, string userId, Vehicle vehicle)
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
            entityToUpdate.LicensePlate = vehicle.LicensePlate;
            entityToUpdate.VehicleType = vehicle.VehicleType;
            entityToUpdate.HourlyWageDriver = vehicle.HourlyWageDriver;
            entityToUpdate.HourlyWagePartner = vehicle.HourlyWagePartner;

            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId; // companyId nullable ise .Value kullan�n
            

            // 4. Repository arac�l���yla g�ncelle ve de�i�iklikleri kaydet.
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
            // ARAMA F�LTRES�
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

            // SAYFALAMA VE VER�Y� �EKME

            var vehicles = await query
                .OrderBy(d => d.VehicleType) // Tutarl� bir s�ralama i�in OrderBy eklemek �nemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VER�Y� DTO'YA D�N��T�RME (MAP'LEME)
            var vehiclesDtos = _mapper.Map<List<VehicleDto>>(vehicles);

            // SONUCU OLU�TURMA
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
            // 2. �nce kullan�c�y� ve �irketini DO�RU mant�kla buluyoruz.
            // Bu sorgu veritaban�na gider, t�m kullan�c�lar� belle�e �ekmez.
            var user = await _repository.GetAsQueryable() // Veya do�rudan DbContext
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Company.ResponsibleUserId.Equals(userId));

            if (user == null || user.CompanyId == null)
            {
                // Kullan�c� yoksa veya �irkete atanmam��sa bo� liste d�nd�r.
                return new List<DropDownDto>();
            }

            var companyId = user.CompanyId;

            // 3. Ara�lar�, veritaban� seviyesinde filtreleyip istedi�imiz formata d�n��t�r�yoruz.
            // _repository'nin VehicleRepository oldu�unu varsay�yorum.
            return await _repository.GetAsQueryable()
                .Where(v => v.CompanyId.Equals(companyId) && !v.IsDeleted) // �nce filtrele
                .Select(v => new DropDownDto { Id = v.Id, Name = v.LicensePlate }) // Sonra se�
                .ToListAsync(); // En son listeye �evir

        }
    }
} 