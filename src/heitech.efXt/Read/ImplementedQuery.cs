using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt
{
    internal class ImplementedQuery<T, Id> : IQuery<T, Id>
        where T : class, IHasId<Id>
        where Id : IEquatable<Id>
    {
        public IQueryable<T> Query => query;
        private readonly IQueryable<T> query;

        public ImplementedQuery(IQueryable<T> query)
        {
            this.query = query;
        }

        public async Task<IReadOnlyList<T>> GetAsListAsync()
        {
            return await query.ToListAsync();
        }

        public async Task<T> GetAsync()
        {
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TResult> PrjectIntoAsync<TResult>(System.Linq.Expressions.Expression<Func<T, TResult>> projection)
        {
            return await query.Select(projection).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> PrjectIntoListAsync<TResult>(System.Linq.Expressions.Expression<Func<T, TResult>> projection)
        {
            return await query.Select(projection).ToListAsync();
        }
    }
}