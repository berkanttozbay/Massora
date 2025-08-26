using Microsoft.EntityFrameworkCore;
using Massora.Common.Repositories;
using Massora.Domain.Entities;
using Massora.DataAccess.Context;

namespace Massora.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly MassoraDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MassoraDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;
            
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.IsDeleted = false;
            }

            await _dbSet.AddRangeAsync(entityList);
            await _context.SaveChangesAsync();
            return entityList;
        }

        public async Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                entity.IsDeleted = true;
                entity.UpdatedDate = DateTime.UtcNow;
            }

            _dbSet.UpdateRange(entityList);
            await _context.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        { 
            entity.UpdatedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }

            _dbSet.UpdateRange(entityList);
            await _context.SaveChangesAsync();
            return entityList;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<T>> GetListAsync()
        {
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhereAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.Where(x => !x.IsDeleted).Where(predicate).ToList());
        }

        public async Task<bool> AnyAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.Where(x => !x.IsDeleted).Any(predicate));
        }

        public async Task<int> CountAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.Where(x => !x.IsDeleted).Count(predicate));
        }
        
        public IQueryable<T> GetAsQueryable()
        {
            return _dbSet.Where(x => !x.IsDeleted).AsQueryable();
        }

        


    }
} 