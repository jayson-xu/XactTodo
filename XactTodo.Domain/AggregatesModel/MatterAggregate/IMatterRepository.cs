using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{
    public interface IMatterRepository : IRepository<Matter>
    {
        Matter Add(Matter matter);

        void Update(Matter matter);

        void Delete(int id);

        Task<Matter> GetAsync(int id);

        IEnumerable<Matter> Find(Expression<Func<Matter, bool>> expression);

        IQueryable<Matter> GetAll();

    }

}
