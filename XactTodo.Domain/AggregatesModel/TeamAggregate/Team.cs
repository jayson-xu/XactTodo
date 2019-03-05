using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.TeamAggregate
{
    public class Team : FullAuditedEntity, IAggregateRoot
    {
        public const int MaxNameLength = 100;
        public const int MaxProposedTagsLength = 500;

        /// <summary>
        /// 小组名称(同一用户创建的小组，名称不得重复)
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 组内提议使用的Tags(多个Tag之间以;号分隔)
        /// </summary>
        [StringLength(MaxProposedTagsLength)]
        public string ProposedTags { get; set; }

        public ICollection<Member> Members { get; set; } = new List<Member>();

        /// <summary>
        /// 组长Id
        /// </summary>
        /// <remarks>组长一定在Members集合中存在，且一定为管理者身份，不允许取消</remarks>
        public int LeaderId { get; set; }
    }
}
