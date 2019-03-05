using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private TodoContext context;

        public UserRepository(TodoContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public void Add(User user)
        {
            context.Users.Add(user);
        }

        public void Update(User User)
        {
            context.Entry(User).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var user = context.Users.Find(id);
            if (user == null)
                throw new KeyNotFoundException($"not found a User via Id:{id}");
            context.Remove(user);
        }

        public User FindById(int id)
        {
            return context.Users.Find(id);
        }

        public IEnumerable<User> Find(Expression<Func<User, bool>> expression)
        {
            var items = context.Users.Local.AsQueryable().Where(expression);
            if(items.Count()==0)
                items = context.Users.Where(expression);
            return items;
        }

        public async Task<User> GetAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            return user;
        }

        public IQueryable<User> GetAll()
        {
            return context.Users.AsQueryable();
        }
    }
}
