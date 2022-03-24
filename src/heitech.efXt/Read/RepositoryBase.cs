using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Read
{
    ///<summary>
    /// A combinaton of the two available Repository types: IReadRepository & IRepository. So you can make use of the full Specification pattern and simpler Queries (like GetById etc.)
    ///</summary>
    public abstract class RepositoryBase<TEntity, TId> : GenericRepository<TEntity, TId>, IReadRepository<TEntity>
        where TEntity : class
    {
        public RepositoryBase(DbContext dbContext) 
            : base(dbContext)
        { }

        public Task<IReadOnlyList<TEntity>> GetListAsync(IReadSpecification<TEntity> specification)
            => specification.RunAsync(Context.Set<TEntity>().AsQueryable());

        public Task<TEntity> GetSingleAsync(IReadSpecification<TEntity> specification)
            => specification.RunSingleAsync(Context.Set<TEntity>().AsQueryable());
    }
}
