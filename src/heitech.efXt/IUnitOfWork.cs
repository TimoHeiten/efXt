using System;
using System.Threading.Tasks;

namespace heitech.efXt
{
    public interface IUnitOfWork
    {
        void Add<T, I>(T one, params T[] more)
            where T : class, IHasId<I>
            where I : IEquatable<I>;

        void Update<T, I>(T one, params T[] more)
            where T : class, IHasId<I>
            where I : IEquatable<I>;

        void Delete<T, I>(T one, params T[] more)
            where T : class, IHasId<I>
            where I : IEquatable<I>;

        void RollbackOne<T, I>(T entity)
            where T : class, IHasId<I>
            where I : IEquatable<I>;

        void RollBack();
        Task SaveAsync();
    }
}