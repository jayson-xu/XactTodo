using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XactTodo.Domain.SeedWork
{
    public interface IRepository<TEntity, TKey> where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TKey id);

        TEntity FindById(TKey id);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        IQueryable<TEntity> GetAll();

    }

    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : IAggregateRoot
    {

    }

}
