using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace heitech.efXt
{
    public interface IRepository<TEntity, TId>
            where TEntity : class
    {
        IQueryable<TEntity> AsQueryable();

        Task<TEntity> GetByIdAsync(TId id);
        Task<TEntity> GetByIdAsync(ISpecification<TEntity> specification);

        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<IReadOnlyList<TEntity>> GetAllByIdAsync(ISpecification<TEntity> specification);
    }
}
