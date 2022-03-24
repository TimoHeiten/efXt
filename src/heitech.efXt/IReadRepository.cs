using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace heitech.efXt
{
    ///<summary>
    /// A Repository that completely relies on the Specification pattern. If you like a more simple one, use the IRepository
    ///</summary>
     public interface IReadRepository<TEntity>
        where TEntity : class
    {
        Task<IReadOnlyList<TEntity>> GetListAsync(IReadSpecification<TEntity> specification);
        Task<TEntity> GetSingleAsync(IReadSpecification<TEntity> specification);
    }
}
