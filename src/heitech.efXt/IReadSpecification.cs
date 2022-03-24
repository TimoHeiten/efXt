using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace heitech.efXt
{
    ///<summary>
    /// Provides Full blown querying capabilities for testable queries with the Specification pattern in combination with IReadRepository
    ///</summary>
    public interface IReadSpecification<TEntity>
        where TEntity : class
    {
        Task<IReadOnlyList<TEntity>> RunAsync(IQueryable<TEntity> query);
        Task<TEntity> RunSingleAsync(IQueryable<TEntity> query);
    }
}
