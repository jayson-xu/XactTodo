using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.TeamAggregate
{
    /// <summary>
    /// 小组成员
    /// </summary>
    public class Member : FullAuditedEntity
    {
        public int TeamId { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        /// <summary>
        /// 是否管理者
        /// </summary>
        /// <remarks>管理者和组长权限相同，但是组长的管理者身份不允许被取消，但前者可以</remarks>
        public bool IsSupervisor { get; set; }

    }
}
