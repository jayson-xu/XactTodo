using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Infrastructure.Repositories
{
    public class MatterRepository : IMatterRepository
    {
        private TodoContext context;

        public MatterRepository(TodoContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public void Add(Matter matter)
        {
            context.Matters.Add(matter);
        }

        public void Update(Matter matter)
        {
            context.Entry(matter).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var matter = context.Matters.Find(id);
            if (matter == null)
                throw new KeyNotFoundException($"not found a Matter via Id:{id}");
            context.Remove(matter);
        }

        public Matter FindById(int id)
        {
            return context.Matters.Find(id);
        }

        public IEnumerable<Matter> Find(Expression<Func<Matter, bool>> expression)
        {
            var items = context.Matters.Local.AsQueryable().Where(expression);
            if(items.Count()==0)
                items = context.Matters.Where(expression);
            return items;
        }

        public async Task<Matter> GetAsync(int id)
        {
            var matter = await context.Matters.FindAsync(id);
            if (matter != null)
            {
                await context.Entry(matter)
                    .Collection(i => i.Evolvements).LoadAsync();
                await context.Entry(matter).Reference(i => i.EstimatedTimeRequired).LoadAsync();
                await context.Entry(matter).Reference(i => i.IntervalPeriod).LoadAsync();
            }
            return matter;
        }

        public IQueryable<Matter> GetAll()
        {
            return context.Matters.AsQueryable();
        }
    }
}
