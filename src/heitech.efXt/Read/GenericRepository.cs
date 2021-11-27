using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Read
{
    public abstract class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class
    {
        protected DbContext Context { get; }
        public GenericRepository(DbContext dbContext)
            => Context = dbContext;

        public abstract Task<TEntity> GetByIdAsync(TId id);

        public IQueryable<TEntity> AsQueryable() => Context.Set<TEntity>().AsQueryable();

        public Task<TEntity> GetByIdAsync(ISpecification<TEntity> specification)
            => Context.Set<TEntity>().FirstOrDefaultAsync(specification.Build());

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
            => await Context.Set<TEntity>().ToListAsync();

        public async Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(ISpecification<TEntity> specification)
            => await Context.Set<TEntity>().Where(specification.Build()).ToListAsync();
    }
}
