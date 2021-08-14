using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace heitech.efXt.Write
{
    internal class UnitOfWork : IUnitOfWork, ICommands
    {
        private readonly DbContext context;
        private IDbContextTransaction activeTransaction;
        public UnitOfWork(DbContext context)
            => this.context = context;

        public void Add<T, I>(T one, params T[] more)
            where T : class, IHasId<I>
            where I : IEquatable<I>
        {
            context.Add(one);
            if (more.Any())
            {
                context.AddRange(more);
            }
        }

        public void Delete<T, I>(T one, params T[] more)
            where T : class, IHasId<I>
            where I : IEquatable<I>
        {
            context.Remove(one);
            if (more.Any())
                context.RemoveRange(more);
        }

        public void Update<T, I>(T one, params T[] more)
            where T : class, IHasId<I>
            where I : IEquatable<I>
        {
            context.Update(one);
            if (more.Any())
                context.UpdateRange(more);
        }

        public void RollbackOne<T, I>(T entity)
            where T : class, IHasId<I>
            where I : IEquatable<I>
        {
            var firstOrDefault = context.ChangeTracker
                                        .Entries()
                                        .Where(x => x.Entity is T)
                                        .FirstOrDefault(x => ((T)x.Entity).Id.Equals(entity.Id));

            firstOrDefault.State = EntityState.Unchanged;
        }

        public void RollBack()
        {
            context.ChangeTracker
                   .Entries()
                   .ToList()
                   .ForEach
                   (
                       x => 
                       {
                           if (x.State == EntityState.Added)
                           {
                                x.State = EntityState.Detached;
                           }
                           else
                           {
                               x.State = EntityState.Unchanged;
                           }
                       }
                   );
        }

        public async Task SaveAsync()
        {
            try
            {
                await context.SaveChangesAsync();
                await activeTransaction?.CommitAsync();
                
            }
            catch (System.Exception)
            {
                if (activeTransaction != null)
                    await activeTransaction.RollbackAsync();
            }
            finally
            {
                if (activeTransaction != null)
                    await activeTransaction.DisposeAsync();

                activeTransaction = null;
            }
        }

        public ICommands WithTransaction()
        {
            this.activeTransaction = this.context.Database.BeginTransaction();
            return this;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return this;
        }
    }
}