using Chat.Application.Presistance.Contracts;
using Chat.Domain.Common;
using Chat.Infrastructe.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Infrastructe.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal DbSet<T> _dbset;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbset = dbContext.Set<T>();
        }   
        public async Task AddAsync(T entity) =>  await _dbset.AddAsync(entity);
        public async Task<bool> DeleteAsync(int id)
        {
            var entity=await _dbset.FindAsync(id);
            if (entity is null) return false;
            _dbset.Remove(entity);
            return true;    
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _dbset.AsNoTracking().ToListAsync();
        
        public async Task<T?> GetAsync(int id)
        {
            var entity = await _dbset.FindAsync(id);
              return (entity is not null)?  entity : null;
        }
        public Task UpdatedAsync(T entity)
        {
            _dbset.Entry(entity).State=EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
