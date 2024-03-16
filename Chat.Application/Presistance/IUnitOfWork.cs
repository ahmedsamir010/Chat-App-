using Chat.Application.Presistance.Contracts;
using Chat.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Presistance
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity> repository<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();
    }
}
