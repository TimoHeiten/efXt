
using System;
using System.Linq.Expressions;

namespace heitech.efXt
{
    ///<summary>
    /// Provides Filtering capabilities for a Query on the Repository calls.
    ///</summary>
    public interface ISpecification<TEntity>
            where TEntity : class
    {
        Expression<Func<TEntity, bool>> Build();
    }
}
