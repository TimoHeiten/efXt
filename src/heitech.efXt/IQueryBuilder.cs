using System;

namespace heitech.efXt
{
    ///<summary>
    /// Use the aquired IQueryBuilder to start the QuerySpecification process
    ///</summary>
    public interface IQueryBuilder<T, Id>
        where T : class, IHasId<Id>
        where Id : IEquatable<Id>
    {
        IQuerySpecification<T, Id> Select();
        IQuerySpecification<T, Id> SelectWithId(Id id);
    }
}