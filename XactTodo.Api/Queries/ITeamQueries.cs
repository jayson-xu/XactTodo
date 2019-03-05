using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Queries
{
    /// <summary>
    /// 小组查询接口
    /// </summary>
    public interface ITeamQueries : IQueries
    {
        /// <summary>
        /// 获取指定Id的小组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Team> GetAsync(int id);

        /// <summary>
        /// 获取指定用户加入的全部小组
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="incCreatedTeams">包含指定用户所创建的小组</param>
        /// <returns></returns>
        Task<IEnumerable<TeamOutline>> GetJoinedTeamsAsync(int userId, bool incCreatedTeams=false);

        /// <summary>
        /// 获取指定用户管理的全部小组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<TeamOutline>> GetManagedTeamsAsync(int userId);

        /// <summary>
        /// 获取指定小组内全部成员
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<IEnumerable<MemberOutline>> GetMembersAsync(int teamId);

    }
}
