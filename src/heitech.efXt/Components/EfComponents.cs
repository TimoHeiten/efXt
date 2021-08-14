using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using heitech.efXt;
using heitech.efXt.Read;
using heitech.efXt.Write;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EfComponents
    {
        ///<summary>
        /// Set allowed Type Combinations if you want to override the default behavior of identifying types with IHasId
        ///</summary>
        public static (Type entityType, Type idType)[] AllowedTypes { get; set; }
        ///<summary>
        /// Register the ef core abstraction and define an action to be performed on adding a scoped DbContext
        ///</summary>
        public static IServiceCollection RegisterEfAbstraction<T>(this IServiceCollection collection, Action<DbContextOptionsBuilder> registerContext)
            where T : DbContext
        {
            collection.AddDbContext<T>(builder => registerContext(builder));
            collection.AddScoped<ICommands, UnitOfWork>(sp => 
            {
                var context = sp.GetService<T>();
                return new UnitOfWork(context);
            });

            collection.AddScoped<IQueryBuilderFactory, QueryBuilderFactory>
            (
                sp => 
                {
                    var dbContext = sp.GetRequiredService<T>();
                    return new QueryBuilderFactory(dbContext)
                    {
                        AllowedCombinations = AllowedTypes ?? FindAllowedTypes<T>()
                    };
                }
            );

            return collection.Scan
            (
                x => x.FromAssemblyOf<IUnitOfWork>()
                .AddClasses(c => c.AssignableTo(typeof(IQueryBuilder<,>)))
            );
        }

        private static (Type hasId, Type id)[] FindAllowedTypes<T>()
        {
            var allowedTypes = new List<(Type hasId, Type id)>();
            var types = typeof(T).Assembly.GetTypes();
            foreach (var t in types)
            {
                var generics = t.GetInterfaces();
                var hasId = generics.FirstOrDefault(@interface => @interface.Name.Contains("IHasId"));
                if (hasId != null)
                {
                    var memberType = hasId.GenericTypeArguments.Single();
                    allowedTypes.Add((t, memberType));
                }
            }
            return allowedTypes.ToArray();
        }
    }
}