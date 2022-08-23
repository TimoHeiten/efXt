using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Read
{
    public abstract class GenericRepository<TDbContext,TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class
        where TDbContext : DbContext
    {
        protected TDbContext Context { get; }

        protected GenericRepository(TDbContext dbContext)
            => Context = dbContext;

        public abstract Task<TEntity> GetByIdAsync(TId id);

        public IQueryable<TEntity> AsQueryable() => Context.Set<TEntity>().AsQueryable();

        public virtual Task<TEntity> GetByAsync(ISpecification<TEntity> specification)
            => Context.Set<TEntity>().FirstOrDefaultAsync(specification.Build());

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
            => await Context.Set<TEntity>().ToListAsync();

        public virtual async Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(ISpecification<TEntity> specification)
            => await Context.Set<TEntity>().Where(specification.Build()).ToListAsync();
    }
}
