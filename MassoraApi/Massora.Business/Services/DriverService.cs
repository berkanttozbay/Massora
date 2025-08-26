using AutoMapper;
using Azure;
using Humanizer;
using Massora.Business.DTOs;
using Massora.Common.Repositories;
using Massora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.AccessControl;

namespace Massora.Business.Services
{
    public class DriverService : IDriverService
    {
        private readonly IRepository<Driver> _repository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public DriverService(IRepository<Driver> repository, IMapper mapper, IHttpClientFactory httpClientFactory)
        {      
            _repository = repository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Driver> AddAsync(Driver driver,string userId)
        {
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;
            // 1. ADIM: AuthAPI'ye göndermek için bir istek gövdesi hazýrla
            var authApiRequest = new
            {
                Email = driver.Email
                // AuthAPI'nin beklediði diðer alanlar...
            };

            // 2. ADIM: AuthAPI'ye kullanýcý oluþturma isteði at
            var client = _httpClientFactory.CreateClient();
            // AuthAPI'nizin kullanýcý oluþturma endpoint'inin adresini yazýn
            var response = await client.PostAsJsonAsync("http://localhost:5139/api/account/register", authApiRequest);

            if (!response.IsSuccessStatusCode)
            {
                // AuthAPI'de kullanýcý oluþturulamazsa hata fýrlat
                throw new Exception("AuthAPI üzerinde kullanýcý oluþturulamadý.");
            }

            // 3. ADIM: AuthAPI'den dönen cevabý oku ve yeni kullanýcý ID'sini al
            var createdUser = await response.Content.ReadFromJsonAsync<AuthUserResponseDto>();
            if (createdUser == null || string.IsNullOrEmpty(createdUser.Id))
            {
                throw new Exception("AuthAPI'den geçerli bir kullanýcý ID'si dönmedi.");
            }
            var newUserIdFromAuth = createdUser.Id;

            // 4. ADIM: MassoraDb'ye kaydedilecek Driver profilini oluþtur
            var newDriver = _mapper.Map<Driver>(driver);

            // 5. ADIM: AuthAPI'den gelen ID'yi ResponsibleUserId'ye ata
            ;

            // Burada CompanyId gibi diðer atamalar da yapýlabilir
            newDriver = new Driver
            {
                // DTO'dan gelen veriler 
                Name = driver.Name,
                Phone = driver.Phone,
                Email = driver.Email,
                BirthDate = driver.BirthDate,
                Gender = driver.Gender,
                VehicleId = driver.VehicleId,
                ResponsibleUserId = newUserIdFromAuth,

                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullanýn
            };


            return await _repository.AddAsync(newDriver);
        }
        

        public async Task<Driver> UpdateAsync(int id , string userId,Driver driver)
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
            entityToUpdate.Name = driver.Name;
            entityToUpdate.Phone = driver.Phone;
            entityToUpdate.Email = driver.Email;
            entityToUpdate.BirthDate = driver.BirthDate;
            entityToUpdate.Gender = driver.Gender;
            entityToUpdate.VehicleId = driver.VehicleId;

            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId; // companyId nullable ise .Value kullanýn
           

            // 4. Repository aracýlýðýyla güncelle ve deðiþiklikleri kaydet.
            return await _repository.UpdateAsync(entityToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<Driver?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Driver>> GetAllAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<IEnumerable<Driver>> GetByCompanyIdAsync(int companyId)
        {
            return await _repository.GetWhereAsync(x => x.CompanyId == companyId);
        }

        public async Task<Driver?> GetByEmailAsync(string email)
        {
            var drivers = await _repository.GetWhereAsync(x => x.Email == email);
            return drivers.FirstOrDefault();
        }

        public async Task<Driver?> GetByPhoneAsync(string phone)
        {
            var drivers = await _repository.GetWhereAsync(x => x.Phone == phone);
            return drivers.FirstOrDefault();
        }
        public async Task<PaginationResultModel<DriverDto>> GetDriversPaginatedAsync(string loggedInUserId ,int pageNumber, int pageSize, string searchTerm)
        {
            var query = _repository.GetAsQueryable();

            query = query.Where(item => item.Company.ResponsibleUserId.Equals(loggedInUserId));
            query = _repository.GetAsQueryable()
                .Include(driver => driver.Company) // <-- EKSÝK OLAN KRÝTÝK SATIR
                .Include(driver => driver.Vehicle);
            // ARAMA FÝLTRESÝ
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim(); 
                query = query.Where(v =>
                    v.Name.ToLower().Contains(term) ||
                    v.Email.ToLower().Contains(term)
                );
            }

            // TOPLAM KAYIT SAYISINI ALMA
            
            var totalCount = await query.CountAsync();

            // SAYFALAMA VE VERÝYÝ ÇEKME
           
            var drivers = await query
                .OrderBy(d => d.Name) // Tutarlý bir sýralama için OrderBy eklemek önemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VERÝYÝ DTO'YA DÖNÜÞTÜRME (MAP'LEME)
            var driverDtos = drivers.Select(driver => new DriverDto
            {
                Id = driver.Id,
                Phone = driver.Phone,
                BirthDate = driver.BirthDate,
                Gender = driver.Gender,
                Name = driver.Name,
                Email = driver.Email,
                CreatedDate = driver.CreatedDate,
                VehicleId = driver.VehicleId,
                VehicleType = driver.Vehicle?.VehicleType, // Ýliþkili araç tipini al
                CompanyId = driver.CompanyId,
                CompanyName = driver.Company?.Name, // Ýliþkili þirketin adýný al
                                                    // ... diðer alanlar
            }).ToList();

            // SONUCU OLUÞTURMA
            var result = new PaginationResultModel<DriverDto>
            {
                Items = driverDtos,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return result;
        }

        public async Task<IEnumerable<DropDownDto>> GetDriversForDropdownAsync(string userId)
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
                .Select(v => new DropDownDto { Id = v.Id, Name = v.Email }) // Sonra seç
                .ToListAsync(); // En son listeye çevir

        }
    }
} 