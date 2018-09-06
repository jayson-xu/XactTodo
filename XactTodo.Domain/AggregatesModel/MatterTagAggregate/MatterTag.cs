using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.MatterTagAggregate
{
    public class MatterTag : BaseEntity, IAggregateRoot
    {
        public int MatterId { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Tag { get; set; }
    }
}
