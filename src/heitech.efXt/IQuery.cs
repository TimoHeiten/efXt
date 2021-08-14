using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt
{
    ///<summary>
    /// The IQuery Interface with access to execution or the Queryable itself
    ///</summary>
    public interface IQuery<T, Id>
        where T : class, IHasId<Id>
        where Id : IEquatable<Id>
    {
        IQueryable<T> Query { get; }
        Task<T> GetAsync();
        Task<IReadOnlyList<T>> GetAsListAsync();
        Task<TResult> PrjectIntoAsync<TResult>(Expression<Func<T, TResult>> projection);
        Task<IReadOnlyList<TResult>> PrjectIntoListAsync<TResult>(Expression<Func<T, TResult>> projection);
    }
}