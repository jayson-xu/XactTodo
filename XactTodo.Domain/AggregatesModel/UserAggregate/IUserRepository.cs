using System;
using System.Collections.Generic;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository: IRepository<User>
    {
    }

}
