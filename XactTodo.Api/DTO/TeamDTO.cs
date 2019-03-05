using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XactTodo.Domain.AggregatesModel.TeamAggregate;

namespace XactTodo.Api.DTO
{
    public class TeamInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小组名称(同一用户创建的小组，名称不得重复)
        /// </summary>
        [Required]
        [StringLength(Team.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 组内提议使用的Tags(多个Tag之间以;号分隔)
        /// </summary>
        [StringLength(Team.MaxProposedTagsLength)]
        public string ProposedTags { get; set; }

    }
}
