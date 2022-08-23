using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace heitech.efXt
{
    ///<summary>
    /// Abstraction for DataAccess with generic and Specification methods
    ///</summary>
    public interface IRepository<TEntity, TId>
            where TEntity : class
    {
        /// <summary>
        /// For special cases operate directly on the IQueryable or create an IQueryObject
        /// </summary>
        IQueryable<TEntity> AsQueryable();

        Task<TEntity> GetByIdAsync(TId id);
        Task<TEntity> GetByAsync(ISpecification<TEntity> specification);

        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(ISpecification<TEntity> specification);
    }
}
