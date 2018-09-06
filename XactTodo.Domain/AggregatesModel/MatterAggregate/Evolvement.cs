using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{
    /// <summary>
    /// 事项进展/评论
    /// </summary>
    public class Evolvement : BaseEntity, ICreationAudited, ISoftDelete
    {
        private const int MaxCommentLength = 500;
        public int MatterId { get; set; }

        [StringLength(MaxCommentLength)]
        public string Comment { get; set; }

        public int CreatorUserId { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
