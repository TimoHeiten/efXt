
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Read
{
    internal class QueryBuilderFactory : IQueryBuilderFactory
    {
        private readonly DbContext context;
        public (Type type, Type id)[] AllowedCombinations;

        public QueryBuilderFactory(DbContext context)
            => this.context = context;

        // allowedtypecombination --> load all types with hasid<T> and add to allowedTypeCombinations or exception

        public IQueryBuilder<T, I> Resolve<T, I>()
            where T : class, IHasId<I>
            where I : IEquatable<I>
        {
            var t = typeof(T);
            var i = typeof(I);

            if (!AllowedCombinations.Any(types => types.Item1 == t && types.Item2 == i))
            {
                throw new InvalidOperationException($"A Querybuilder for type {t} and id {i} could not be established due to a non allowed combination");
            }
            return new GenericQueryBuilder<T, I>(context);
        } 
    }
}