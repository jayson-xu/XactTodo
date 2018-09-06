using System;
using System.Collections.Generic;
using System.Text;

namespace XactTodo.Domain.SeedWork
{
    public abstract class BaseEntity<TPrimaryKey>
    {
        //[Key]
        public virtual TPrimaryKey Id { get; set; }

    }

    public class BaseEntity : BaseEntity<int>
    {
    }

}
