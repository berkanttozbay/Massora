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
            // 1. ADIM: AuthAPI'ye g�ndermek i�in bir istek g�vdesi haz�rla
            var authApiRequest = new
            {
                Email = driver.Email
                // AuthAPI'nin bekledi�i di�er alanlar...
            };

            // 2. ADIM: AuthAPI'ye kullan�c� olu�turma iste�i at
            var client = _httpClientFactory.CreateClient();
            // AuthAPI'nizin kullan�c� olu�turma endpoint'inin adresini yaz�n
            var response = await client.PostAsJsonAsync("http://localhost:5139/api/account/register", authApiRequest);

            if (!response.IsSuccessStatusCode)
            {
                // AuthAPI'de kullan�c� olu�turulamazsa hata f�rlat
                throw new Exception("AuthAPI �zerinde kullan�c� olu�turulamad�.");
            }

            // 3. ADIM: AuthAPI'den d�nen cevab� oku ve yeni kullan�c� ID'sini al
            var createdUser = await response.Content.ReadFromJsonAsync<AuthUserResponseDto>();
            if (createdUser == null || string.IsNullOrEmpty(createdUser.Id))
            {
                throw new Exception("AuthAPI'den ge�erli bir kullan�c� ID'si d�nmedi.");
            }
            var newUserIdFromAuth = createdUser.Id;

            // 4. ADIM: MassoraDb'ye kaydedilecek Driver profilini olu�tur
            var newDriver = _mapper.Map<Driver>(driver);

            // 5. ADIM: AuthAPI'den gelen ID'yi ResponsibleUserId'ye ata
            ;

            // Burada CompanyId gibi di�er atamalar da yap�labilir
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
                CompanyId = companyId, // companyId nullable ise .Value kullan�n
            };


            return await _repository.AddAsync(newDriver);
        }
        

        public async Task<Driver> UpdateAsync(int id , string userId,Driver driver)
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
            entityToUpdate.Name = driver.Name;
            entityToUpdate.Phone = driver.Phone;
            entityToUpdate.Email = driver.Email;
            entityToUpdate.BirthDate = driver.BirthDate;
            entityToUpdate.Gender = driver.Gender;
            entityToUpdate.VehicleId = driver.VehicleId;

            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId; // companyId nullable ise .Value kullan�n
           

            // 4. Repository arac�l���yla g�ncelle ve de�i�iklikleri kaydet.
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
                .Include(driver => driver.Company) // <-- EKS�K OLAN KR�T�K SATIR
                .Include(driver => driver.Vehicle);
            // ARAMA F�LTRES�
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

            // SAYFALAMA VE VER�Y� �EKME
           
            var drivers = await query
                .OrderBy(d => d.Name) // Tutarl� bir s�ralama i�in OrderBy eklemek �nemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VER�Y� DTO'YA D�N��T�RME (MAP'LEME)
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
                VehicleType = driver.Vehicle?.VehicleType, // �li�kili ara� tipini al
                CompanyId = driver.CompanyId,
                CompanyName = driver.Company?.Name, // �li�kili �irketin ad�n� al
                                                    // ... di�er alanlar
            }).ToList();

            // SONUCU OLU�TURMA
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
                .Select(v => new DropDownDto { Id = v.Id, Name = v.Email }) // Sonra se�
                .ToListAsync(); // En son listeye �evir

        }
    }
} 