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
        Task<Matter> GetAsync(int id);

    }

}
