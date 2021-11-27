using heitech.efXt.Write;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Components
{
    public static class Factory
    {
        ///<summary>
        /// Create a new Instance of IUnitOfWork (remember to dispose after saving all the changes)
        ///</summary>
        public static IUnitOfWork CreateUnitOfWork(this DbContext dbContext)
            => new UnitOfWork(dbContext);
    }
}
