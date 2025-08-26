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
                // Kullan�c� bulunamad� veya bir �irkete atanmam��sa hata f�rlat
                throw new Exception("Kullan�c� bir �irkete atanmam��.");
            }

            // 2. Yeni Vehicle entity'sini olu�tur
            var newPartnerCompany = new PartnerCompany
            {
                // DTO'dan gelen veriler
                Name = partnerCompany.Name,
                ContactPhone = partnerCompany.ContactPhone,
                ContactEmail = partnerCompany.ContactEmail,
                Address = partnerCompany.Address,
                

                // Backend'de bulunan CompanyId'yi ata
                CompanyId = companyId, // companyId nullable ise .Value kullan�n
            };

            // Arac�n eklenmesi
            return await _repository.AddAsync(newPartnerCompany);
        }

        public async Task<PartnerCompany> UpdateAsync(int id ,string userId,PartnerCompany partnerCompany)
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
            entityToUpdate.Name = partnerCompany.Name;
            entityToUpdate.ContactPhone = partnerCompany.ContactPhone;
            entityToUpdate.ContactEmail = partnerCompany.ContactEmail;
            entityToUpdate.Address = partnerCompany.Address;
            
            // Backend'de bulunan CompanyId'yi ata
            entityToUpdate.CompanyId = companyId; // companyId nullable ise .Value kullan�n
            

            // 4. Repository arac�l���yla g�ncelle ve de�i�iklikleri kaydet.
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
                .Include(driver => driver.Company); // <-- EKS�K OLAN KR�T�K SATIR

            // ARAMA F�LTRES�
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

            // SAYFALAMA VE VER�Y� �EKME

            var partnerCompanies = await query
                .OrderBy(d => d.Name) // Tutarl� bir s�ralama i�in OrderBy eklemek �nemlidir.
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // VER�Y� DTO'YA D�N��T�RME (MAP'LEME)
            var partnerCompanyDtos = _mapper.Map<List<PartnerCompanyDto>>(partnerCompanies);

            // SONUCU OLU�TURMA
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
            // 3. �irketin partner �irketlerini al�yoruz.
            var partnerCompanies = await _repository.GetWhereAsync(x => x.CompanyId == user.CompanyId);
            // 4. Partner �irketleri DropDownDto'ya d�n��t�r�yoruz.
            return partnerCompanies.Select(pc => new DropDownDto
            {
                Id = pc.Id,
                Name = pc.Name
            });
        }
    }
} 