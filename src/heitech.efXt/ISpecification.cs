
using System;
using System.Linq.Expressions;

namespace heitech.efXt
{
     public interface ISpecification<TEntity>
            where TEntity : class
    {
        Expression<Func<TEntity, bool>> Build();
    }
}
