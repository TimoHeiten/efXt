using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace heitech.efXt.Read
{
    internal sealed class GenericQueryBuilder<T, Id> : IQueryBuilder<T, Id>, IQuerySpecification<T, Id>
        where T : class, IHasId<Id>
        where Id : IEquatable<Id>
    {
        private readonly DbContext context;
        private Expression<Func<T, bool>> filter;
        private Expression<Func<T, bool>> idExpression;
        private IQueryable<T> query;

        public GenericQueryBuilder(DbContext context)
        {
            this.context = context;
        }

        #region QueryBuilder
        public IQuerySpecification<T, Id> SelectWithId(Id id)
            =>  _select(x => x.Id.Equals((Id)id));

        public IQuerySpecification<T, Id> Select()
            => _select(x => true);

        private IQuerySpecification<T, Id> _select(Expression<Func<T, bool>> idExpression)
        {
            query = context.Set<T>().AsQueryable();
            this.idExpression = idExpression;
            return this;
        }
        #endregion

        public IQuery<T, Id> Create()
        {
            return new ImplementedQuery<T, Id>(query);
        }

        public IQuerySpecification<T, Id> Filter(Expression<Func<T, bool>> filter)
        {
            query = query.Where(filter);
            return this;
        }

        public IQuerySpecification<T, Id> Include(params string[] includes)
        {
            foreach (var include in includes)
                query = query.Include(include);

            return this;
        }

        public IQuerySpecification<T, Id> Include<Tkey>(Expression<Func<T, Tkey>> include)
        {
            query = query.Include(include);
            return this;
        }

        public IQuerySpecification<T, Id> OrderBy<TKey>(Func<T, TKey> order)
        {
            query = query.OrderBy(order).AsQueryable();
            return this;
        }

        public IQuerySpecification<T, Id> OrderByDescending<TKey>(Func<T, TKey> order)
        {
            query = query.OrderByDescending(order).AsQueryable();
            return this;
        }
    }
}