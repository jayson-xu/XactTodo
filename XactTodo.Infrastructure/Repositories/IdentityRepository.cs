using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private TodoContext context;

        public IdentityRepository(TodoContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public void Add(Identity identity)
        {
            context.Identities.Add(identity);
        }

        public void Update(Identity Identity)
        {
            context.Entry(Identity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var identity = context.Identities.Find(id);
            if (identity == null)
                throw new KeyNotFoundException($"not found a Identity via Id:{id}");
            context.Remove(identity);
        }

        public Identity FindById(int id)
        {
            return context.Identities.Find(id);
        }

        public IEnumerable<Identity> Find(Expression<Func<Identity, bool>> expression)
        {
            var items = context.Identities.Local.AsQueryable().Where(expression);
            if(items.Count()==0)
                items = context.Identities.Where(expression);
            return items;
        }

        public async Task<Identity> GetAsync(int id)
        {
            var identity = await context.Identities.FindAsync(id);
            return identity;
        }

        public IQueryable<Identity> GetAll()
        {
            return context.Identities.AsQueryable();
        }
    }
}
