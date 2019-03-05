using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Queries
{
    public class TeamOutline
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 组内提议使用的Tags
        /// </summary>
        public string[] ProposedTags { get; set; }


    }

    public class MemberOutline
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public bool IsSupervisor { get; set; }

    }

    public class Team : TeamOutline
    {
        public Team()
        {
            this.Members = new List<MemberOutline>();
        }

        /// <summary>
        /// 组员
        /// </summary>
        public List<MemberOutline> Members { get; }
    }

}
