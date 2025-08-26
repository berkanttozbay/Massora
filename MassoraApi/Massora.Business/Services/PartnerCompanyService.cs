using AutoMapper;
using Massora.Business.DTOs;
using Massora.Common.Repositories;
using Massora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Massora.Business.Services
{
    public class PartnerCompanyService : IPartnerCompanyService
    {
        private readonly IRepository<PartnerCompany> _repository;
        private readonly IMapper _mapper;

        public PartnerCompanyService(IRepository<PartnerCompany> repository , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PartnerCompany> AddAsync(PartnerCompany partnerCompany,string userId)
        {
            var users = _repository.GetAsQueryable();
            var user = users.Include(u => u.Company).FirstOrDefault(u => u.Company.ResponsibleUserId.Equals(userId));
            var companyId = user.CompanyId;

            if (user == null || user.CompanyId == null)
            {
                // Kullanýcý bulunamadý veya bir þirkete atanmamýþsa hata fýrlat
                throw new Exception("Kullanýcý bir þirkete atanmamýþ.");
            }

            // 2. Yeni Vehicle entity'sini oluþtur
            var newPartnerCompany = new PartnerCompany
            {
                // DTO'dan gelen veriler
                Name = partnerCompany.Name,
                ContactPhone = partnerCompany.ContactPhone,
                ContactEmail = partnerCompany.ContactEmail,
                Address = partnerCompany.Address,
                

                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullanýn
            };

            // Aracýn eklenmesi
            return await _repository.AddAsync(newPartnerCompany);
        }

        public async Task<PartnerCompany> UpdateAsync(int id ,string userId,PartnerCompany partnerCompany)
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
            entityToUpdate.Name = partnerCompany.Name;
            entityToUpdate.ContactPhone = partnerCompany.ContactPhone;
            entityToUpdate.ContactEmail = partnerCompany.ContactEmail;
            entityToUpdate.Address = partnerCompany.Address;
            
            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId; // companyId nullable ise .Value kullanýn
            

            // 4. Repository aracýlýðýyla güncelle ve deðiþiklikleri kaydet.
            return await _repository.UpdateAsync(entityToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<PartnerCompany?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PartnerCompany>> GetAllAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<IEnumerable<PartnerCompany>> GetByCompanyIdAsync(int companyId)
        {
            return await _repository.GetWhereAsync(x => x.CompanyId == companyId);
        }

        public async Task<PaginationResultModel<PartnerCompanyDto>> GetPartnerCompaniesPaginatedAsync(string loggedInUserId, int pageNumber, int pageSize, string searchTerm)
        {
            var query = _repository.GetAsQueryable();

            query = query.Where(item => item.Company.ResponsibleUserId.Equals(loggedInUserId));
            query = _repository.GetAsQueryable()
                .Include(driver => driver.Company); // <-- EKSÝK OLAN KRÝTÝK SATIR

            // ARAMA FÝLTRESÝ
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(v =>
                    v.Name.ToLower().Contains(term) ||
                    v.Address.ToLower().Contains(term)
                );
            }

            // TOPLAM KAYIT SAYISINI ALMA

            var totalCount = await query.CountAsync();

            // SAYFALAMA VE VERÝYÝ ÇEKME

            var partnerCompanies = await query
                .OrderBy(d => d.Name) // Tutarlý bir sýralama için OrderBy eklemek önemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VERÝYÝ DTO'YA DÖNÜÞTÜRME (MAP'LEME)
            var partnerCompanyDtos = _mapper.Map<List<PartnerCompanyDto>>(partnerCompanies);

            // SONUCU OLUÞTURMA
            var result = new PaginationResultModel<PartnerCompanyDto>
            {
                Items = partnerCompanyDtos,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return result;
        }

        public async Task<IEnumerable<DropDownDto>> GetPartnerCompaniesForDropdownAsync(string userId)
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
            // 3. Þirketin partner þirketlerini alýyoruz.
            var partnerCompanies = await _repository.GetWhereAsync(x => x.CompanyId == user.CompanyId);
            // 4. Partner þirketleri DropDownDto'ya dönüþtürüyoruz.
            return partnerCompanies.Select(pc => new DropDownDto
            {
                Id = pc.Id,
                Name = pc.Name
            });
        }
    }
} 