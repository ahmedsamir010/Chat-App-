using Chat.Application.Presistance;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Common;
using Chat.Infrastructe.Data;
namespace Chat.Infrastructe.Repositories
{
    public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
    {
        private bool _disposed = false;
        private Dictionary<Type,object> _repositories=new Dictionary<Type,object>();
        public async Task<int> CompleteAsync() => await dbContext.SaveChangesAsync();
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var typeEntity=typeof(TEntity);
            if(!_repositories.TryGetValue(typeEntity,out var repository))
            {
                repository = new GenericRepository<TEntity>(dbContext);
                _repositories.Add(typeEntity, repository);
            }
            return (IGenericRepository<TEntity>)repository;
        }
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await dbContext.DisposeAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
