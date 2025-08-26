using Massora.Common.Repositories;
using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _repository;

        public CompanyService(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task<Company> AddAsync(Company company)
        {
            return await _repository.AddAsync(company);
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            return await _repository.UpdateAsync(company);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task<IEnumerable<Company>> GetByResponsibleUserIdAsync(int responsibleUserId)
        {
            return await _repository.GetWhereAsync(x => x.ResponsibleUserId.Equals(responsibleUserId));
        }

        public IQueryable<Company> GetAsQueryable()
        {
            return _repository.GetAsQueryable();

        }
    }
} 