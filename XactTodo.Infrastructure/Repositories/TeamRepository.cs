using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private TodoContext context;

        public TeamRepository(TodoContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public void Add(Team team)
        {
            context.Teams.Add(team);
        }

        public void Update(Team team)
        {
            context.Entry(team).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var team = context.Teams.Find(id);
            if (team == null)
                throw new KeyNotFoundException($"not found a Team via Id:{id}");
            context.Remove(team);
        }

        public Team FindById(int id)
        {
            return context.Teams.Find(id);
        }

        public IEnumerable<Team> Find(Expression<Func<Team, bool>> expression)
        {
            var items = context.Teams.Local.AsQueryable().Where(expression);
            if(items.Count()==0)
                items = context.Teams.Where(expression);
            return items;
        }

        public async Task<Team> GetAsync(int id)
        {
            var team = await context.Teams.FindAsync(id);
            if (team != null)
            {
                await context.Entry(team)
                    .Collection(i => i.Members).LoadAsync();
            }
            return team;
        }

        public IQueryable<Team> GetAll()
        {
            return context.Teams.AsQueryable();
        }

    }
}
