using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.TeamAggregate
{
    public interface ITeamRepository: IRepository<Team>
    {
        Task<Team> GetAsync(int id);

    }

}
