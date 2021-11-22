using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace heitech.efXt.Write
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;
        public UnitOfWork(DbContext context)
            => this.context = context;

        public void Add<T>(T one, params T[] more)
            where T : class
        {
            context.Add(one);
            if (more.Any())
                context.AddRange(more);
        }

        public void Delete<T>(T one, params T[] more)
            where T : class
        {
            context.Remove(one);
            if (more.Any())
                context.RemoveRange(more);
        }

        public void Update<T>(T one, params T[] more)
           where T : class
        {
            context.Update(one);
            if (more.Any())
                context.UpdateRange(more);
        }

        public void RollbackOne<T>(T entity, Func<T, bool> predicate)
           where T : class
        {
            var firstOrDefault = context.ChangeTracker
                                        .Entries()
                                        .Where(x => x.Entity is T)
                                        .FirstOrDefault(x => predicate(((T)x.Entity)));

            RollbackState(firstOrDefault);
        }

        public void RollBackAll()
        {
            context.ChangeTracker
                   .Entries()
                   .ToList()
                   .ForEach(RollbackState);
        }

        private static void RollbackState(EntityEntry  entry)
        {
            if (entry == null)
                return;

            EntityState state = entry.State;
            entry.State = state == EntityState.Added 
                          ? EntityState.Detached 
                          : EntityState.Unchanged;
        }

        public async Task SaveAsync()
            => await context.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await SaveAsync();
    }
}