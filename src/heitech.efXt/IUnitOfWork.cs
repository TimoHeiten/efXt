using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace heitech.efXt
{
    /// <summary>
    /// Represents all Commands to interact with the database and have a 
    /// Transaction scope in the Application before committing the changes to the database
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        void Add<T>(T one)
            where T : class;
        void  AddMany<T>(IEnumerable<T> many)
            where T : class;

        void Update<T>(T one)
            where T : class;
        void UpdateMany<T>(IEnumerable<T> many)
            where T : class;

        void Delete<T>(T one)
            where T : class;
        void DeleteMany<T>(IEnumerable<T> many)
            where T : class;

        ///<summary>
        /// Resets all changes on this entity, so it will not be commited to the database on Save
        ///</summary>
        void RollbackOne<T>(Func<T, bool> predicate)
            where T : class;

        ///<summary>
        /// Resets all changes on all changed entities, so it will not be commited to the database on Save
        ///</summary>
        void RollBackAll();

        ///<summary>
        /// Commit all the changes made to any entities since the start of the UnitOfWork
        ///</summary>
        Task SaveAsync();
    }
}