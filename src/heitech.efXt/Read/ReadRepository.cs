using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Read
{
    ///<summary>
    /// Use this generic ReadRepository to access Specification specific Repository capabilities
    ///</summary>
    public class ReadRepository<TEntity, TId> : IReadRepository<TEntity, TId>
        where TEntity : class
    {
        protected DbContext Context { get; }
        public ReadRepository(DbContext dbContext)
            => Context = dbContext;

        public Task<IReadOnlyList<TEntity>> GetListAsync(IReadSpecification<TEntity> specification)
            => specification.RunAsync(Context.Set<TEntity>().AsQueryable());

        public Task<TEntity> GetSingleAsync(IReadSpecification<TEntity> specification)
            => specification.RunSingleAsync(Context.Set<TEntity>().AsQueryable());
    }
}
