using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace heitech.efXt.Write
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        public UnitOfWork(DbContext context)
            => _context = context;

        private DbSet<T> Set<T>() where T : class  => _context.Set<T>();

        public void Add<T>(T one) where T : class
            => Set<T>().Add(one);

        public void Delete<T>(T one) where T : class
            => Set<T>().Remove(one);

        public void Update<T>(T one) where T : class
            => Set<T>().Update(one);

        public void AddMany<T>(IEnumerable<T> many) where T : class
            => Set<T>().AddRange(many);

        public void UpdateMany<T>(IEnumerable<T> many) where T : class
            => Set<T>().UpdateRange(many);

        public void DeleteMany<T>(IEnumerable<T> many) where T : class
            => Set<T>().RemoveRange(many);

        public void RollbackOne<T>(Func<T, bool> predicate)
           where T : class
        {
            var firstOrDefault = _context.ChangeTracker
                                        .Entries()
                                        .Where(x => x.Entity is T)
                                        .FirstOrDefault(x => predicate(((T)x.Entity)));

            RollbackState(firstOrDefault);
        }

        public void RollBackAll()
        {
            _context.ChangeTracker
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
            => await _context.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(disposing: false);
            #pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
            #pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) 
                return;

            _context.SaveChanges();
            _context?.Dispose();
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            await SaveAsync();
            await _context.DisposeAsync();
        }
    }
}