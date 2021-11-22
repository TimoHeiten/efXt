using System;
using System.Threading.Tasks;

namespace heitech.efXt
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        void Add<T>(T one, params T[] more)
            where T : class;

        void Update<T>(T one, params T[] more)
            where T : class;

        void Delete<T>(T one, params T[] more)
            where T : class;

        void RollbackOne<T>(T entity, Func<T, bool> predicate)
            where T : class;

        void RollBackAll();
        Task SaveAsync();
    }
}