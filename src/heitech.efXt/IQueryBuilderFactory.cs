using System;

namespace heitech.efXt
{
    ///<summary>
    /// The entry point to build Queries for your application. Use Resolve to get a builder that can then create specifications for your queries.
    ///</summary>
    public interface IQueryBuilderFactory
    {
        ///<summary>
        /// Resolve T:EntityType with I:Id. A QueryBuilder is returned that allows to 
        ///</summary>
        IQueryBuilder<T, I> Resolve<T, I>()
            where T : class, IHasId<I>
            where I : IEquatable<I>;
    }
}