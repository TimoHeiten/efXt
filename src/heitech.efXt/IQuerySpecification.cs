using System;
using System.Linq.Expressions;

namespace heitech.efXt
{
    ///<summary>
    /// Queryspecification pattern. Abstract way to define a query with the usual entity framework query operators. For more control use Create and then the Queryable (might impact your test capabilities)
    ///</summary>
    public interface IQuerySpecification<T, Id> 
        where T : class, IHasId<Id>
        where Id : IEquatable<Id>
    {
        ///<summary>
        /// Build the IQuery with the Result operators
        ///</summary>
        IQuery<T, Id> Create();
        ///<summary>
        /// OrderBy operator
        ///</summary>
        IQuerySpecification<T, Id> OrderBy<TKey>(Func<T, TKey> order);
        ///<summary>
        /// OrderByDescending operator
        ///</summary>
        IQuerySpecification<T, Id> OrderByDescending<TKey>(Func<T, TKey> order);

        ///<summary>
        /// Analogous to Linq.Where Operator
        ///</summary>
        IQuerySpecification<T, Id> Filter(Expression<Func<T, bool>> filter);

         ///<summary>
        /// Abstract Include of Ef core via property expresssions
        ///</summary>
        IQuerySpecification<T, Id> Include(params string[] includes);
        ///<summary>
        /// Abstract Include of Ef core via Linq.Expression Trees
        ///</summary>
        IQuerySpecification<T, Id> Include<Tkey>(Expression<Func<T, Tkey>> include);
    }
}