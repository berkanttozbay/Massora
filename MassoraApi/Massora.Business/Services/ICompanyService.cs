using Massora.Domain.Entities;

namespace Massora.Business.Services
{
    public interface ICompanyService
    {
        Task<Company> AddAsync(Company company);
        Task<Company> UpdateAsync(Company company);
        Task DeleteAsync(int id);
        Task<Company?> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<IEnumerable<Company>> GetByResponsibleUserIdAsync(int responsibleUserId);
        IQueryable<Company> GetAsQueryable();
    }
} 