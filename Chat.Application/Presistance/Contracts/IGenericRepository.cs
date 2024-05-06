using System.Linq.Expressions;
namespace Chat.Application.Presistance.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<bool> AddAsync(T entity);
        Task UpdatedAsync(T entity);    
        Task<bool> DeleteAsync(int id);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllWithIncludeAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
    }
}
