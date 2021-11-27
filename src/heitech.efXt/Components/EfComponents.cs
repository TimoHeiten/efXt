using System;
using heitech.efXt;
using heitech.efXt.Write;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EfComponents
    {
        ///<summary>
        /// Register the ef core abstraction and define an action to be performed on adding a scoped DbContext
        ///</summary>
        public static IServiceCollection RegisterEfAbstraction<T>(this IServiceCollection collection, Action<DbContextOptionsBuilder> registerContext)
            where T : DbContext
        {
            collection.AddDbContext<T>(builder => registerContext(builder));
            collection.AddScoped<IUnitOfWork, UnitOfWork>(sp => 
            {
                var context = sp.GetService<T>();
                return new UnitOfWork(context);
            });

            return collection.Scan
            (
                x => x.FromApplicationDependencies().AddClasses(c => c.AssignableTo(typeof(IRepository<,>)))
            );
        }
    }
}